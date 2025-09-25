using CLI.UI;
using FileRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI app...");
IUserRepository userRepository = new UserFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository();
ISubForumRepository? subForumRepository = new SubForumFileRepository();
IPostVoteRepository? postVoteRepository = new PostVoteFileRepository();
ICommentVoteRepository? commentVoteRepository =
    new CommentVoteFileRepository();

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository,
    subForumRepository, postVoteRepository, commentVoteRepository);
await cliApp.StartAsync();