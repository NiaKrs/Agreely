using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;
using Agreely.Services.Interfaces;


namespace Agreely.Services.Services
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepo;
        private readonly ICommitmentRepository _commitmentRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IActivityLogService _activityLogService;

        public VoteService(IVoteRepository voteRepo, ICommitmentRepository commitmentRepo, IGroupRepository groupRepo, IActivityLogService activityLogService)
        {
            _voteRepo = voteRepo;
            _commitmentRepo = commitmentRepo;
            _groupRepo = groupRepo;
            _activityLogService = activityLogService;
        }

        public void CastOrUpdateVote(CastVoteRequest request)
        {
            var vote = _voteRepo.GetVote(request.CommitmentVersionId, request.UserId);
            var eventType = EventTypeValue.VoteCast;
            if (vote == null)
            {
                _voteRepo.CastVote(new AlignmentVote
                {
                    CommitmentVersionId = request.CommitmentVersionId,
                    UserId = request.UserId,
                    Vote = request.Vote
                });
            }
            else
            {
                _voteRepo.UpdateVote(new AlignmentVote
                {
                    CommitmentVersionId = request.CommitmentVersionId,
                    UserId = request.UserId,
                    Vote = request.Vote
                });
                eventType = EventTypeValue.VoteChanged;
            }

            var commitmentVersion = _commitmentRepo.GetCurrentVersion(request.CommitmentId);
            var title = commitmentVersion?.Title ?? "Unknown";
            _activityLogService.LogEvent(request.GroupId, request.UserId, eventType, eventType == EventTypeValue.VoteCast ? $"Voted {request.Vote} on \"{title}\"" : $"Changed vote to {request.Vote}");

            var allVotes = _voteRepo.GetVotesByVersion(request.CommitmentVersionId);
            var agreeCount = allVotes.Count(v => v.Vote == VoteValue.Agree);
            var memberCount = _groupRepo.GetMemberCount(request.GroupId);

            var previousStatus = _commitmentRepo.GetCommitmentById(request.CommitmentId)?.Status;

            var newStatus = (agreeCount == memberCount && memberCount > 0)
                ? CommitmentStatus.Active
                : CommitmentStatus.Pending;

            _commitmentRepo.UpdateCommitmentStatus(request.CommitmentId, newStatus);

            if (previousStatus != newStatus)  
                _activityLogService.LogEvent(request.GroupId, request.UserId, EventTypeValue.StatusChanged, $"\"{title}\" status changed to {newStatus}");
        }

        public VoteValue? GetUserVote(int commitmentVersionId, int userId)
        {
            var vote = _voteRepo.GetVote(commitmentVersionId, userId);
            return vote?.Vote;
        }

        public VoteCountResponse GetVoteCounts(int commitmentVersionId)
        {
            var votes = _voteRepo.GetVotesByVersion(commitmentVersionId);
            return new VoteCountResponse
            {
                Agree = votes.Count(v => v.Vote == VoteValue.Agree),
                Neutral = votes.Count(v => v.Vote == VoteValue.Neutral),
                Disagree = votes.Count(v => v.Vote == VoteValue.Disagree),
                Total = votes.Count
            };
        }

    }
}
