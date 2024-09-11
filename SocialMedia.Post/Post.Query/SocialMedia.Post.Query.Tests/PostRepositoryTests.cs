using FluentAssertions;
using FluentAssertions.Execution;
using SocialMedia.Post.Query.Domain.Entities;
using SocialMedia.Post.Query.Infrastructure.Repositories;

namespace SocialMedia.Post.Query.Tests
{
    public class PostRepositoryTests : DatabaseTestBase
    {
        private PostRepository _postRepository;
        private CommentRepository _commentRepository;

        private Stack<Action> _cleanUpStack = [];

        [SetUp]
        public void Setup()
        {
            _postRepository = new PostRepository(ConnectionString);
            _commentRepository = new CommentRepository(ConnectionString);
        }

        [TearDown]
        public void TearDown()
        {
            while(_cleanUpStack.TryPop(out var cleanUp))
            {
                cleanUp();
            }
        }

        [Test]
        public async Task ShouldCreateNewPost()
        {
            // Arrange
            var post = new PostEntity()
            { 
                Author = "newyorktimes",
                PostId = Guid.NewGuid(),
                DatePosted = DateTime.Now,
                Message = "New post"
            };

            // Act
            await _postRepository.CreateAsync(post);
            _cleanUpStack.Push(async () => await _postRepository.DeleteAsync(post.PostId));

            // Assert
            var persistedPost = await _postRepository.GetByIdAsync(post.PostId);

            using (new AssertionScope())
            {
                persistedPost.Author.Should().Be(post.Author);
                persistedPost.DatePosted.Should().BeCloseTo(post.DatePosted, TimeSpan.FromMilliseconds(1));
                persistedPost.PostId.Should().Be(post.PostId);
                persistedPost.Message.Should().Be(post.Message);
                persistedPost.Comments.Should().BeEmpty();
                persistedPost.TotalLikes.Should().Be(0);
            }
        }

        [Test]
        public async Task ShouldOnlyUpdatePostMessage()
        {
            // Arrange
            var originalPost = await CreatePostAsync();
            var alteredPost = new PostEntity
            {
                PostId = originalPost.PostId,
                Author = originalPost.Author + "brand new author",
                TotalLikes = originalPost.TotalLikes + 100,
                Message = originalPost.Message + " small addendum",
                DatePosted = originalPost.DatePosted.AddDays(-1)
            };

            // Act
            await _postRepository.UpdateAsync(alteredPost);

            // Assert
            var persistedPost = await _postRepository.GetByIdAsync(alteredPost.PostId);

            using (new AssertionScope())
            {
                persistedPost.Author.Should().Be(originalPost.Author);
                persistedPost.DatePosted.Should().BeCloseTo(originalPost.DatePosted, TimeSpan.FromMilliseconds(1));
                persistedPost.PostId.Should().Be(originalPost.PostId);
                persistedPost.Message.Should().Be(alteredPost.Message);
                persistedPost.Comments.Should().BeEmpty();
                persistedPost.TotalLikes.Should().Be(originalPost.TotalLikes);
            }
        }

        [Test]
        public async Task ShouldDeletePost()
        {
            // Arrange
            var originalPost = await CreatePostAsync();

            // Act
            await _postRepository.DeleteAsync(originalPost.PostId);

            // Assert
            (await _postRepository.GetByIdAsync(originalPost.PostId)).Should().BeNull();
        }

        [Test]
        public async Task ShouldListAllPosts()
        {
            // Arrange
            var expectedIds = await CreatePostsAsync(5);

            // Act
            var actualIds = (await _postRepository.ListAllAsync()).Select(x => x.PostId).ToArray();
            
            // Assert
            actualIds.Should().BeEquivalentTo(expectedIds);
        }

        [Test]
        public async Task ShouldListAllPostsByAuthor()
        {
            // Arrange
            var author01 = "author01";
            var author02 = "author02";

            var expectedIds = await CreatePostsAsync(3, author01);
            var _ = await CreatePostsAsync(2, author02);

            // Act
            var actualIds = (await _postRepository.ListByAuthorAsync(author01)).Select(x => x.PostId).ToArray();

            // Assert
            actualIds.Should().BeEquivalentTo(expectedIds);
        }

        [Test]
        public async Task ShouldListAllPostsWithLikes()
        {
            // Arrange
            int totalLikes = 3;
            var postIds = (await CreatePostsAsync(3));
            var expectedIds = new Guid[] { postIds[0], postIds[2] };

            for(int i = 0; i < totalLikes; i++)
            {
                await _postRepository.IncrementLikesAsync(expectedIds[0]);
                await _postRepository.IncrementLikesAsync(expectedIds[1]);
            }

            // Act
            var actualIds = (await _postRepository.ListWithLikesAsync(totalLikes)).Select(x => x.PostId).ToArray();

            // Assert
            actualIds.Should().BeEquivalentTo(expectedIds);
        }

        [Test]
        public async Task ShouldListAllPostsWithComments()
        {
            // Arrange
            var postIds = (await CreatePostsAsync(3);
            var idsWithComments = postIds.Take(2).ToArray();

            foreach (var id in idsWithComments)
            {
                await _commentRepository.CreateAsync(idsWithComments[0]);
            }

            // Act
            var actualIds = (await _postRepository.ListWithLikesAsync(totalLikes)).Select(x => x.PostId).ToArray();

            // Assert
            actualIds.Should().BeEquivalentTo(idsWithComments);
        }

        private async Task<PostEntity> CreatePostAsync()
        {
            var post = new PostEntity()
            {
                Author = "newyorktimes",
                PostId = Guid.NewGuid(),
                DatePosted = DateTime.Now,
                Message = "New post"
            };
            await _postRepository.CreateAsync(post);

            _cleanUpStack.Push(async () => await _postRepository.DeleteAsync(post.PostId));

            return post;
        }

        private async Task<Guid[]> CreatePostsAsync(int quantity, string author=null)
        {
            List<Guid> guids = [];

            for(int i=0; i < Math.Max(quantity, 0); i++)
            {
                var post = new PostEntity()
                {
                    PostId = Guid.NewGuid(),
                    DatePosted = DateTime.Now,
                    Author = author ?? "author" + i,
                    Message = "New post " + i
                };

                await _postRepository.CreateAsync(post);

                _cleanUpStack.Push(async () => await _postRepository.DeleteAsync(post.PostId));

                guids.Add(post.PostId);
            }

            return guids.ToArray();
        }
    }
}