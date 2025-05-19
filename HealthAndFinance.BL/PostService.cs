using Dapper;
using HealthAndFinance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HealthAndFinance.BL
{
    public class PostService
    {
        private DapperContext _context;
        public PostService(DapperContext context)
        {
            _context = context;
        }
        
        private const string GET_USER_POSTS = @"
        SELECT 
            p.PostId, p.Title, p.Content, p.PostTypeId, 
            p.CreationDate, p.ParentPostID,
            u.UserId, u.FirstName, u.LastName,
            pt.PostTypeId AS PostTypesId, pt.PostType
        FROM Posts p
        INNER JOIN Users u ON p.UserId = u.UserID
        LEFT JOIN PostsTypes pt ON p.PostTypeId = pt.PostTypeId
        WHERE p.UserId = @UserId
        ORDER BY p.CreationDate DESC";

        public async Task<List<Post>> GetUserPosts(int userId)
        {
            var posts = await _context.QueryAsync<Post, User, PostsType, Post>(
                GET_USER_POSTS,
                (post, author, postType) =>
                {
                    post.Author = author;
                    post.PostType = postType;
                    return post;
                },
                new { UserId = userId },
                splitOn: "UserID,PostTypesId");

            return posts.ToList();
        }

        private const string CREATE_POST_SQL = @"
        INSERT INTO Posts 
                (UserId, Title, Content, PostTypeId, CreationDate)
        VALUES (@UserId, @Title, @Content, @PostTypeId, @CreationDate);
        SELECT CAST(SCOPE_IDENTITY() as int);";


        public async Task<int> CreatePostAsync(Post post)
        {
            if(post == null) throw new ArgumentNullException(nameof(post));

            using var connection = _context.CreateConnection();
            connection.Open();

            var postId = await connection.ExecuteScalarAsync<int>(CREATE_POST_SQL, post);
            return postId;
        }


        private const string GET_POSTS_SQL = @"
            SELECT 
            p.PostId, p.Title, p.Content, p.PostTypeId, 
                        p.CreationDate, p.ParentPostID,
                        u.UserId, u.FirstName, u.LastName,
                        pt.PostTypeId AS PostTypesId, pt.PostType
                    FROM Posts p
                    INNER JOIN Users u ON p.UserId = u.UserID
                    LEFT JOIN PostsTypes pt ON p.PostTypeId = pt.PostTypeId
                    WHERE p.PostId = @PostId
                    ORDER BY p.CreationDate DESC";                
                    
        public async Task<List<Post>> GetPostById(int postId)
        {
            var posts = await _context.QueryAsync<Post, User, PostsType, Post>(
                GET_POSTS_SQL,
                (post, author, postType) =>
                {
                    post.Author = author;
                    post.PostType = postType;
                    return post;
                },
                new { PostId = postId },
                splitOn: "UserID,PostTypesId");

            return posts.ToList();
        }
    }
}
