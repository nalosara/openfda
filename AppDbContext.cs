using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Text.Json;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Drug> Drugs { get; set; }
    public DbSet<AdverseEvent> AdverseEvents { get; set; }
    public DbSet<OpenFda> OpenFda { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        var listStringConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default), 
            v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>() 
        );

        var listStringComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2), 
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), 
            c => c != null ? c.ToList() : new List<string>() 
        );

        modelBuilder.Entity<Drug>()
            .Property(d => d.Purpose)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<Drug>()
            .Property(d => d.InactiveIngredient)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<Drug>()
            .Property(d => d.Warnings)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<Drug>()
            .Property(d => d.DosageAndAdministration)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<Drug>()
            .Property(d => d.IndicationsAndUsage)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<Drug>()
            .Property(d => d.PackageLabelPrincipalDisplayPanel)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<Drug>()
            .Property(d => d.ActiveIngredient)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<OpenFda>()
            .Property(o => o.BrandName)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<OpenFda>()
            .Property(o => o.GenericName)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<OpenFda>()
            .Property(o => o.ManufacturerName)
            .HasConversion(listStringConverter)
            .Metadata.SetValueComparer(listStringComparer);

        modelBuilder.Entity<Drug>()
            .HasOne(d => d.OpenFda)
            .WithMany(o => o.Drugs)
            .HasForeignKey(d => d.OpenFdaId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
