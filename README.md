# IMS

## Añadir Entity Framework Core

Instalar los siguiente paquetes NuGet:
- Microsoft.EntityFrameworkCore.Design, v7.x.x
- Microsoft.EntityFrameworkCore.SqlServer, v7.x.x
- Microsoft.EntityFrameworkCore.Tools, v7.x.x
 
En los siguientes proyectos:
- IMS.Plugins.EFCoreSqlServer
- IMS.WebApp



## Añadir Identity

Trabajamos sobre el proyecto IMS.WebApp, a los paquetes ya existentes,
- Microsoft.EntityFrameworkCore.Design, v7.x.x
- Microsoft.EntityFrameworkCore.SqlServer, v7.x.x
- Microsoft.EntityFrameworkCore.Tools, v7.x.x

Le añadimos los siguientes:
- Microsoft.AspNetCore.Identity.EntityFrameworkCore, v6.x.x
- Microsoft.AspNetCore.Identity.UI, v6.x.x
- 

Al tratarse de un proyecto en .NET 6, instalaremos la version 6.x.x

### Program.cs

```
    //Configure EF Core for Identity
    builder.Services.AddDbContext<AccountDbContext>(options =>
    {
        options.UseSqlServer(constr);
    });

    //Configure Identity
    builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
    }).AddEntityFrameworkStores<AccountDbContext>();
```

```

    app.UseAuthentication();
    app.UseAuthorization();

```

### App.razor
Añadir CascadingAuthenticationState al fichero App.razor

```
    <CascadingAuthenticationState>
        <Router AppAssembly="@typeof(App).Assembly" >
            <Found Context="routeData">
                <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
                <FocusOnNavigate RouteData="@routeData" Selector="h1" />
            </Found>
            <NotFound>
                <PageTitle>Not found</PageTitle>
                <LayoutView Layout="@typeof(MainLayout)">
                    <p role="alert">Sorry, there's nothing at this address.</p>
                </LayoutView>
            </NotFound>
        </Router>
    </CascadingAuthenticationState>
</code>


```

### Ejecutar migraciones
``` 
    Add-Migration -Context AccountDbContext Init-Identity
``` 

``` 
    Update-Database -Context AccountDbContext Init-Identity
``` 

### Identity Scaffolding

- Add > New Scaffolded Item > Identity
- Check Login and Register page.

