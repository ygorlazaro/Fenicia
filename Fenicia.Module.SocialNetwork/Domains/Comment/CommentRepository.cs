using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Comment.Interfaces;

/// <summary>
/// Fenicia.Module.SocialNetwork/Domains/Comment/CommentRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

public class CommentRepository(DefaultContext context) : Repository<CommentModel>(context), ICommentRepository;