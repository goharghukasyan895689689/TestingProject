global using NUnit.Framework;
namespace TestingProject
{
    // User related models
    public class CreateUserRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUserInfoRequest
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    // Product related models
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    // Order related models
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    // Result classes
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string ErrorMessage { get; set; }
    }

    public class CreateUserResult : ServiceResult<User>
    {
        public Guid? UserId { get; set; }
        public User User { get; set; }
    }

    public class LoginResult : ServiceResult<User>
    {
        public string Token { get; set; }
        public User User { get; set; }
    }

    public class CreateProductResult : ServiceResult<Product>
    {
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }

    public class CreateOrderResult : ServiceResult<Order>
    {
        public Order Order { get; set; }
    }

    // Mock service classes (would be replaced with actual implementations)
    public class UserService
    {
        public CreateUserResult CreateUser(CreateUserRequest request)
        {
            // Mock implementation
            if (!request.Email.Contains("@"))
                throw new ArgumentException("Invalid email format");

            if (request.Password.Length < 8)
            {
                return new CreateUserResult
                {
                    Success = false,
                    Errors = { "Password does not meet security requirements" }
                };
            }

            return new CreateUserResult
            {
                Success = true,
                UserId = Guid.NewGuid(),
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                }
            };
        }
    }

    public class AuthenticationService
    {
        private readonly Dictionary<string, int> _failedAttempts = new Dictionary<string, int>();
        private readonly HashSet<string> _lockedAccounts = new HashSet<string>();

        public LoginResult Login(LoginRequest request)
        {
            if (_lockedAccounts.Contains(request.Email))
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Account is locked"
                };
            }

            // Mock successful login for specific credentials
            if (request.Email == "user@test.com" && request.Password == "correctpassword")
            {
                _failedAttempts.Remove(request.Email);
                return new LoginResult
                {
                    Success = true,
                    Token = "mock-jwt-token",
                    User = new User { Email = request.Email }
                };
            }

            // Track failed attempts
            _failedAttempts[request.Email] = _failedAttempts.GetValueOrDefault(request.Email, 0) + 1;

            if (_failedAttempts[request.Email] >= 5)
            {
                _lockedAccounts.Add(request.Email);
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Account is locked"
                };
            }

            return new LoginResult { Success = false };
        }

        public async Task LockUserAccount(string email)
        {
            _lockedAccounts.Add(email);
            await Task.CompletedTask;
        }
    }

    public class UserInfoService
    {
        private readonly Dictionary<Guid, User> _users = new Dictionary<Guid, User>();

        public void CreateTestUser(Guid userId, string email, string firstName, string lastName)
        {
            _users[userId] = new User
            {
                Id = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
        }

        public User GetUserInfo(Guid userId)
        {
            return _users.GetValueOrDefault(userId);
        }

        public ServiceResult<User> UpdateUserInfo(UpdateUserInfoRequest request)
        {
            if (_users.ContainsKey(request.UserId))
            {
                var user = _users[request.UserId];
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;

                return new ServiceResult<User> { Success = true, Data = user };
            }

            return new ServiceResult<User> { Success = false };
        }
    }

    public class PasswordService
    {
        private readonly Dictionary<Guid, string> _userPasswords = new Dictionary<Guid, string>();

        public void CreateTestUser(Guid userId, string password)
        {
            _userPasswords[userId] = password;
        }

        public ServiceResult<bool> ForcePasswordChange(Guid userId, string newPassword)
        {
            if (newPassword.Length < 8)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Errors = { "Password does not meet security requirements" }
                };
            }

            if (_userPasswords.ContainsKey(userId) && _userPasswords[userId] == newPassword)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Errors = { "New password cannot be the same as current password" }
                };
            }

            _userPasswords[userId] = newPassword;
            return new ServiceResult<bool> { Success = true };
        }

        public bool VerifyPassword(Guid userId, string password)
        {
            return _userPasswords.ContainsKey(userId) && _userPasswords[userId] == password;
        }
    }

    public class ProductService
    {
        public CreateProductResult CreateProduct(CreateProductRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Product name cannot be empty");

            if (request.Price < 0)
            {
                return new CreateProductResult
                {
                    Success = false,
                    Errors = { "Price cannot be negative" }
                };
            }

            return new CreateProductResult
            {
                Success = true,
                ProductId = Guid.NewGuid(),
                Product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Category = request.Category,
                    Price = request.Price,
                    Description = request.Description
                }
            };
        }
    }

    public class ProductSearchService
    {
        private readonly List<Product> _products = new List<Product>();

        public void AddTestProducts()
        {
            _products.AddRange(new[]
            {
                new Product { Name = "Gaming Laptop", Category = "Electronics", Price = 1200m },
                new Product { Name = "Office Laptop", Category = "Electronics", Price = 800m },
                new Product { Name = "Coffee Mug", Category = "Kitchen", Price = 15m }
            });
        }

        public List<Product> SearchByName(string searchTerm)
        {
            return _products.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
        }

        public List<Product> SearchByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        }

        public List<Product> SearchByCategory(string category)
        {
            return _products.Where(p => p.Category == category).ToList();
        }
    }

    public class OrderService
    {
        public CreateOrderResult CreateOrder(CreateOrderRequest request)
        {
            if (!request.Items.Any())
            {
                return new CreateOrderResult
                {
                    Success = false,
                    Errors = { "Order must contain at least one item" }
                };
            }

            if (request.Items.Any(item => item.Quantity <= 0))
            {
                return new CreateOrderResult
                {
                    Success = false,
                    Errors = { "Item quantity must be greater than zero" }
                };
            }

            var totalAmount = request.Items.Sum(item => item.Quantity * item.Price);

            return new CreateOrderResult
            {
                Success = true,
                Order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Items = request.Items,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }

    public class OrderStatusService
    {
        private readonly Dictionary<Guid, Order> _orders = new Dictionary<Guid, Order>();

        public void CreateTestOrder(Guid orderId, OrderStatus status)
        {
            _orders[orderId] = new Order
            {
                Id = orderId,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };
        }

        public ServiceResult<bool> UpdateOrderStatus(Guid orderId, OrderStatus newStatus)
        {
            if (!_orders.ContainsKey(orderId))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    ErrorMessage = "Order not found"
                };
            }

            var order = _orders[orderId];

            // Validate status transition (simplified logic)
            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    ErrorMessage = "Invalid status transition"
                };
            }

            order.Status = newStatus;
            return new ServiceResult<bool> { Success = true };
        }

        public OrderStatus GetOrderStatus(Guid orderId)
        {
            return _orders[orderId].Status;
        }

        public Order GetOrder(Guid orderId)
        {
            return _orders[orderId];
        }

        private bool IsValidStatusTransition(OrderStatus current, OrderStatus target)
        {
            // Simplified transition validation
            return (current, target) switch
            {
                (OrderStatus.Pending, OrderStatus.Processing) => true,
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                _ => false
            };
        }
    }
}