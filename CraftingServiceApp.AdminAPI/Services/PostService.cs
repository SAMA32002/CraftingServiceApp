using AutoMapper;
using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Helpers;
using CraftingServiceApp.AdminAPI.Interfaces;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace CraftingServiceApp.AdminAPI.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public PostService(ApplicationDbContext context, IMapper mapper, ILogger<PostService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<PostDto>> GetAllPostsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Posts.AsQueryable();

                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Where(p => p.Title.Contains(queryOptions.Search) || p.Description.Contains(queryOptions.Search));
                }

                query = queryOptions.SortField.ToLower() switch
                {
                    "id" => queryOptions.SortDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                    "title" => queryOptions.SortDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                    "createdat" => queryOptions.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                    _ => queryOptions.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                };

                var totalCount = await query.CountAsync();

                var posts = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var postDtos = _mapper.Map<IEnumerable<PostDto>>(posts);

                return new PagedResult<PostDto>
                {
                    Items = postDtos,
                    TotalCount = totalCount,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all posts.");
                throw;
            }
        }

        public async Task<PostDto> GetPostByIdAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            return post == null ? null : _mapper.Map<PostDto>(post);
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetPostCountAsync()
        {
            return await _context.Posts.CountAsync();
        }
    }
}
