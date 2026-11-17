# TP2 et TP3 - Frameworks de développement ASP.NET Core

Application ASP.NET Core MVC réalisée dans le cadre des TP2 et TP3.

Le projet présente l'utilisation de trois approches de persistance de données dans une même application :

- SQL Server avec Entity Framework Core pour le catalogue ;
- SQL Server avec un second DbContext pour les locations ;
- MongoDB pour les avis des clients.

## Auteur

- **Baha Eddine Manai**
- Classe : **GL3**

## Technologies utilisées

- ASP.NET Core MVC
- .NET 10
- C#
- Entity Framework Core 10
- SQL Server LocalDB
- MongoDB 8
- MongoDB.Driver
- Razor
- Bootstrap
- LINQ

## Fonctionnalités

### Catalogue SQL

- liste des films ;
- ajout d'un film ;
- modification d'un film ;
- suppression d'un film ;
- affichage des détails d'un film ;
- relation entre film et genre ;
- tri par nom ou par genre ;
- pagination ;
- affichage du nom du genre ;
- champ de disponibilité du film.

### Gestion des genres

- liste des genres ;
- ajout d'un genre ;
- données de départ chargées depuis un fichier JSON ;
- relation un-à-plusieurs entre genre et film.

### Gestion des clients

- liste des clients ;
- ajout d'un client ;
- gestion du nom ;
- gestion de l'adresse email ;
- gestion du type d'abonnement ;
- affichage du pourcentage de remise ;
- utilisation de `Include` pour charger le type d'abonnement ;
- utilisation de `Select` pour comparer une projection SQL légère ;
- affichage du SQL généré.

### Gestion des locations

- chaque film peut être loué par un client ;
- modification de la disponibilité dans la base catalogue ;
- création d'une ligne de location dans la base location ;
- affichage du bouton Louer uniquement pour un film disponible ;
- utilisation d'un second DbContext ;
- utilisation de deux bases SQL Server séparées.

### Avis MongoDB

- récupération des avis d'un film ;
- ajout d'un avis ;
- calcul de la note moyenne ;
- note entre 1 et 5 ;
- commentaire ;
- tags ;
- indicateur spoiler ;
- langue de l'avis ;
- date de création ;
- affichage des avis sur la page Details du film.

## Architecture des données

### Base catalogue

La base principale est `CinemaDb`.

```text
CinemaDb
├── Genres
└── Movies
```

Le contexte principal est `ApplicationDbContext`.

Il contient les DbSet suivants :

- `Genres`
- `Movies`

### Base location

La base de location est `CinemaRentalDb`.

```text
CinemaRentalDb
└── rental
    ├── Customers
    ├── MembershipTypes
    └── Rentals
```

Le contexte de location est `RentalDbContext`.

Il contient les DbSet suivants :

- `Customers`
- `MembershipTypes`
- `Rentals`

### Base MongoDB

MongoDB utilise la base `CinemaReviews`.

```text
CinemaReviews
└── movieReviews
```

Chaque avis contient notamment :

- `movieId`
- `rating`
- `comment`
- `tags`
- `spoiler`
- `language`
- `createdAt`

## Relations Entity Framework Core

### Relation Genre - Movie

Un genre possède plusieurs films :

```text
Genre 1 ---- * Movie
```

Chaque film possède un `GenreId` et une navigation vers `Genre`.

### Relation MembershipType - Customer

Un type d'abonnement possède plusieurs clients :

```text
MembershipType 1 ---- * Customer
```

### Relation Customer - Rental

Un client peut avoir plusieurs locations :

```text
Customer 1 ---- * Rental
```

Le champ `MovieId` de `Rental` est une référence logique vers l'autre base. Il ne possède pas de navigation EF vers `Movie`, car les données sont séparées dans deux bases SQL.

## Structure du projet

```text
ASPCoreFirstApp/
├── Controllers/
│   ├── CustomerController.cs
│   ├── GenreController.cs
│   ├── HomeController.cs
│   └── MovieController.cs
├── Data/
│   └── Seed/
│       └── genres.json
├── Migrations/
│   ├── Catalog/
│   ├── Rental/
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   ├── ApplicationDbContext.cs
│   ├── Customer.cs
│   ├── Genre.cs
│   ├── MembershipType.cs
│   ├── Movie.cs
│   ├── MovieReview.cs
│   ├── Rental.cs
│   ├── RentalDbContext.cs
│   └── VM/
├── Services/
│   ├── MongoReviewService.cs
│   └── RentalService.cs
├── Views/
│   ├── Customer/
│   ├── Genre/
│   ├── Home/
│   ├── Movie/
│   └── Shared/
├── wwwroot/
├── appsettings.json
├── Program.cs
└── ASPCoreFirstApp.csproj
```

## Prérequis

Avant de lancer le projet, vérifier que les outils suivants sont installés :

- Visual Studio 2022 ou Visual Studio Code ;
- SDK .NET 10 ;
- SQL Server LocalDB ;
- SQL Server Management Studio ou Azure Data Studio ;
- MongoDB Community Server ;
- MongoDB Shell `mongosh`.

## Configuration SQL Server

Les chaînes de connexion sont configurées dans `appsettings.json`.

La base catalogue utilise :

```text
Server=(localdb)\mssqllocaldb
Database=CinemaDb
```

La base location utilise :

```text
Server=(localdb)\mssqllocaldb
Database=CinemaRentalDb
```

## Configuration MongoDB

La connexion locale configurée est :

```text
mongodb://localhost:27017
```

La base utilisée est :

```text
CinemaReviews
```

MongoDB n'est pas lié par clé étrangère aux tables SQL. Cette séparation permet de comparer une base relationnelle structurée avec une base documentaire flexible.

## Installation

Ouvrir un terminal dans le dossier du projet :

```powershell
cd "C:\Users\bahae\OneDrive\Bureau\ASPCoreFirstApp"
```

Restaurer les packages :

```powershell
dotnet restore
```

Compiler le projet :

```powershell
dotnet build
```

## Application des migrations

Il existe deux contextes Entity Framework Core. L'option `--context` est obligatoire pour chaque opération EF.

Appliquer les migrations du catalogue :

```powershell
dotnet ef database update --context ApplicationDbContext
```

Appliquer les migrations de la base de location :

```powershell
dotnet ef database update --context RentalDbContext
```

Créer une nouvelle migration du catalogue :

```powershell
dotnet ef migrations add NomMigration --context ApplicationDbContext --output-dir Migrations/Catalog
```

Créer une nouvelle migration de la location :

```powershell
dotnet ef migrations add NomMigration --context RentalDbContext --output-dir Migrations/Rental
```

Lister les migrations du catalogue :

```powershell
dotnet ef migrations list --context ApplicationDbContext
```

Lister les migrations de location :

```powershell
dotnet ef migrations list --context RentalDbContext
```

## Lancement de l'application

```powershell
dotnet run
```

Ouvrir ensuite l'adresse affichée après :

```text
Now listening on:
```

Exemple :

```text
http://localhost:5013
```

## Pages principales

| Page | Adresse |
|---|---|
| Accueil | `/` |
| Liste des films | `/Movie` |
| Ajouter un film | `/Movie/Create` |
| Détails d'un film | `/Movie/Details/{id}` |
| Modifier un film | `/Movie/Edit/{id}` |
| Supprimer un film | `/Movie/Delete/{id}` |
| Liste des genres | `/Genre` |
| Ajouter un genre | `/Genre/Create` |
| Liste des clients | `/Customer` |
| Ajouter un client | `/Customer/Create` |

## Seed Data

Le projet contient plusieurs données initiales.

Les genres sont lus depuis :

```text
Data/Seed/genres.json
```

Ils sont copiés automatiquement dans le dossier de sortie grâce à la configuration du fichier `.csproj`.

Le contexte catalogue initialise notamment :

- Action ;
- Drame ;
- Science-fiction ;
- Inception ;
- Gladiator ;
- Interstellar.

Le contexte location initialise notamment :

- Standard ;
- Premium ;
- Gold ;
- Amal Ben Salah ;
- Youssef Trabelsi.

## Différence entre Include et Select

### Avec Include

```csharp
var customers = await _db.Customers
    .AsNoTracking()
    .Include(c => c.MembershipType)
    .ToListAsync();
```

Cette requête charge les clients avec leurs types d'abonnement.

### Avec Select

```csharp
var customers = await _db.Customers
    .AsNoTracking()
    .Select(c => new
    {
        c.Name,
        c.Email,
        MembershipType = c.MembershipType!.Name,
        c.MembershipType!.DiscountPercent
    })
    .ToListAsync();
```

Cette requête récupère uniquement les colonnes utiles à l'affichage.

### Effet de AsNoTracking

`AsNoTracking()` indique à Entity Framework Core de ne pas suivre les objets récupérés.

Cette option est adaptée aux pages d'affichage :

- moins de mémoire utilisée ;
- moins de suivi d'état ;
- meilleure efficacité pour les lectures ;
- les objets ne sont pas préparés automatiquement pour une modification.

## Sécurité des formulaires

Les actions POST utilisent :

```csharp
[ValidateAntiForgeryToken]
```

Les formulaires Razor utilisent les Tag Helpers pour insérer automatiquement le jeton anti-falsification.

Les propriétés acceptées par les formulaires sont limitées avec `[Bind]` afin de réduire les risques de sur-postage.

## Vérification MongoDB

Se connecter à MongoDB :

```powershell
mongosh "mongodb://localhost:27017"
```

Afficher les avis :

```javascript
use CinemaReviews
db.movieReviews.find({ movieId: 1 }).pretty()
```

Calculer la moyenne :

```javascript
db.movieReviews.aggregate([
  { $match: { movieId: 1 } },
  { $group: { _id: "$movieId", moyenne: { $avg: "$rating" } } }
])
```

## Vérification SQL Server

Dans SQL Server, vérifier les genres :

```sql
SELECT Id, Name
FROM dbo.Genres;
```

Vérifier les films :

```sql
SELECT Id, Name, GenreId, IsAvailable
FROM dbo.Movies;
```

Vérifier les types d'abonnement :

```sql
SELECT Id, Name, DiscountPercent
FROM rental.MembershipTypes;
```

Vérifier les clients :

```sql
SELECT
    c.Id,
    c.Name,
    c.Email,
    m.Name AS MembershipType,
    m.DiscountPercent
FROM rental.Customers c
JOIN rental.MembershipTypes m
    ON c.MembershipTypeId = m.Id;
```

## Tests fonctionnels

Avant l'envoi du projet, vérifier les opérations suivantes :

1. L'application démarre avec `dotnet run`.
2. La page d'accueil s'affiche.
3. La liste des films affiche les noms des genres.
4. Un nouveau film peut être ajouté.
5. Un film peut être modifié.
6. Un film peut être supprimé avec confirmation.
7. Le tri par nom fonctionne.
8. Le tri par genre fonctionne.
9. La pagination fonctionne.
10. Un nouveau genre peut être ajouté.
11. Un nouveau client peut être ajouté.
12. La liste des clients affiche l'abonnement.
13. Un film disponible peut être loué.
14. Un film loué n'est plus disponible.
15. Une ligne est ajoutée dans `rental.Rentals`.
16. Un avis peut être ajouté depuis la page Details.
17. Les avis MongoDB s'affichent correctement.
18. La moyenne des notes est recalculée.

## Points importants pour la présentation

- SQL Server sert principalement au catalogue et aux locations.
- MongoDB sert aux avis de structure variable.
- Deux bases SQL sont utilisées avec deux DbContext.
- Les migrations doivent être appliquées séparément avec `--context`.
- `MovieId` dans `Rental` est une référence logique vers l'autre base.
- Une opération touchant les deux bases n'est pas atomique entre les deux bases.
- La compensation remet `IsAvailable` à `true` si l'ajout de la location échoue.
- `Include` charge une relation alors que `Select` récupère uniquement les champs utiles.

## Remarques

Le projet contient actuellement des avertissements NuGet concernant certaines dépendances. Ils n'empêchent pas la compilation ni l'exécution, mais ils doivent être pris en compte avant une utilisation en production.

Les dossiers `bin/` et `obj/` ne doivent pas être envoyés dans GitHub. Ils sont exclus par le fichier `.gitignore`.
