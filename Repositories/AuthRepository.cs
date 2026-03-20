using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task AddDefaultCategoriesAsync(int userId)
        {
            var defaultCategories = new List<Category>
            {
                new() { CategoryName = "Food",          UserId = userId },
                new() { CategoryName = "Transport",     UserId = userId },
                new() { CategoryName = "Rent",          UserId = userId },
                new() { CategoryName = "Entertainment", UserId = userId },
                new() { CategoryName = "Shopping",      UserId = userId },
                new() { CategoryName = "Bills",         UserId = userId },
                new() { CategoryName = "Health",        UserId = userId }
            };
            _context.Categories.AddRange(defaultCategories);
            await _context.SaveChangesAsync();
        }
    }
}