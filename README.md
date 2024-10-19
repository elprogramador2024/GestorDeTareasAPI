# Gestor de Tareas API

Este proyecto es una API desarrollada en **ASP.NET Core** con conexión a base de datos **PostgreSQL**, utilizando **Entity Framework Core** para la gestión de datos y **JWT** para la autenticación de usuarios.

## Requisitos

1. **.NET Core 6.0** instalado.
2. **PostgreSQL 12** instalado.

3. Debes instalar **PostgreSQL 12** y agregar la cadena de conexión correspondiente en el archivo `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=127.0.0.1;Database=GestorTareas;Port=5432;User Id=postgres;Password=123;"
}
```

Debes agregar los parámetros de `Server`, `Database`, `Port`, `User Id` y `Password` según tu configuración local.

## Migraciones de Base de Datos (Code-First)

Para aplicar las migraciones y crear las tablas correspondientes en la base de datos, sigue estos pasos:

1. Instalar Entity Framework Core (EF Core) de manera global en tu entorno de desarrollo
   ```bash
   dotnet tool install --global dotnet-ef --version 3.1.2
   ```

1. Generar la migración inicial:
   ```bash
   dotnet ef migrations add MigracionInicial --project GestorDeTareas
   ```

2. Aplicar la migración a la base de datos:
   ```bash
   dotnet ef database update --project GestorDeTareas
   ```
   
## Configuración de Roles y Usuario Administrador

La API incluye una configuración para crear roles predeterminados y un usuario administrador por defecto cuando se ejecuta por primera vez. Esta configuración asegura que el sistema esté listo para gestionar usuarios y permisos.

## Roles

* Administrador
* Supervisor
* Empleado

## Usuario

usuario : admin
password: Admin052@

## API Publicada

## API .NET
https://gestordetareasapi-alkc.onrender.com
