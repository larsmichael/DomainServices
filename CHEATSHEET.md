# Domain Services Cheat Sheet

## Creating a new entity type
To create a new entity type, inherit from one of the generic abstract entity classes.

```csharp
public class Product : BaseNamedEntity<Guid>
{
    public Product(Guid id, string name) : base(id, name)
    {
    }

    public virtual decimal Price { get; set; }
}
```

## Creating a new repository abstraction
To create a new repository abstraction, extend the generic repository interfaces. 

```csharp
public interface IProductRepository : IRepository<Product, Guid>,
    IDiscreteRepository<Product, Guid>,
    IUpdatableRepository<Product, Guid>
{
    bool ContainsName(string name);
}
```

## Creating a new repository
To create a new repository, implement the repository interface. The generic `JsonRepository` is a repository that persists entities in a JSON file.

```csharp
public class ProductRepository : JsonRepository<Product, Guid>, IProductRepository
{
    public ProductRepository(string filePath) : base(filePath)
    {
    }

    public bool ContainsName(string name)
    {
        return GetAll().Any(product => product.Name.Equals(name));
    }
}
```

## Creating a new service
To create a new service, inherit from one of the generic abstract service classes.

```csharp
public class ProductService : BaseUpdatableDiscreteService<Product, Guid>
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public ProductService(IProductRepository repository, ILogger logger) : base(repository, logger)
    {
        _repository = repository;
    }

    public override void Add(Product product, ClaimsPrincipal? user = null)
    {
        if (_repository.ContainsName(product.Name))
        {
            throw new ArgumentException($"There is already a product with the name '{product.Name}'.", nameof(product));
        }

        base.Add(product, user);
    }
}
```
## Putting it all together
Instantiate a repository and inject it into the service using constructor injection. Add a few entities.

```csharp
var productRepository = new ProductRepository("products.json");
var productService = new ProductService(productRepository);
productService.Add(new Product(Guid.NewGuid(), "Coke") { Price = 1.35M });
productService.Add(new Product(Guid.NewGuid(), "Fanta") { Price = 1.85M });
```

## Using a service in a controller
To use the service in an ASP.NET controller, inject the service (or repository) using constructor injection.

```csharp
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(IProductRepository productRepository)
    {
        _productService = new ProductService(productRepository);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> Get(Guid id) => Ok(_productService.Get(id));
}
```

