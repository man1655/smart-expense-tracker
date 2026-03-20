using ExpenseTracker.API.DTOs.Category;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using ExpenseTracker.API.Services.Interfaces;

namespace ExpenseTracker.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync(int userId)
        {
            var categories = await _categoryRepository.GetAllAsync(userId);
            return categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
        }

        public async Task<(bool Success, string Message)> CreateAsync(int userId, CategoryRequestDto dto)
        {
            var exists = await _categoryRepository.ExistsAsync(dto.CategoryName, userId);
            if (exists) return (false, "Category already exists");

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                UserId = userId
            };

            await _categoryRepository.CreateAsync(category);
            return (true, "Category created");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int userId, int id, CategoryRequestDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id, userId);
            if (category == null) return (false, "Category not found");

            category.CategoryName = dto.CategoryName;
            await _categoryRepository.UpdateAsync(category);
            return (true, "Category updated");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int userId, int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id, userId);
            if (category == null) return (false, "Category not found");

            await _categoryRepository.DeleteAsync(category);
            return (true, "Category deleted");
        }
    }
}