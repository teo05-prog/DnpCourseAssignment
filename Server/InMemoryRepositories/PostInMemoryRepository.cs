﻿using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private List<Post> posts = new List<Post>();

    public PostInMemoryRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        posts.AddRange(new[]
        {
            new Post
            {
                Id = 1, Title = "Welcome to the Forum!",
                Body = "This is the first post. Welcome everyone!", UserId = 1
            },
            new Post
            {
                Id = 2, Title = "C# Programming Tips",
                Body =
                    "Let's discuss some useful C# programming tips and tricks.",
                UserId = 2
            },
            new Post
            {
                Id = 3, Title = "Web Development Discussion",
                Body =
                    "Share your thoughts on modern web development frameworks.",
                UserId = 3
            }
        });
    }

    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);

        posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetManyAsync()
    {
        return posts.AsQueryable();
    }
}