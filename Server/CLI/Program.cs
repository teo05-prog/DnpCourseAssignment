using CLI.UI;
using InMemoryRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI app...");
IUserRepository userRepository = new UserInMemoryRepository();
ICommentRepository commentRepository = new CommentInMemoryRepository();
IPostRepository postRepository = new PostInMemoryRepository();
ISubForumRepository? subForumRepository = new SubForumInMemoryRepository();
IPostVoteRepository? postVoteRepository = new PostVoteInMemoryRepository();
ICommentVoteRepository? commentVoteRepository =
    new CommentVoteInMemoryRepository();

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository,
    subForumRepository, postVoteRepository, commentVoteRepository);
await cliApp.StartAsync();