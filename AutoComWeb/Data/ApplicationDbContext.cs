using System;
using System.Collections.Generic;
using AutoComWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Mechanic> Mechanics { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<SparePart> SpareParts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<WorkOrder> WorkOrders { get; set; }

    public virtual DbSet<WorkOrderPart> WorkOrderParts { get; set; }

    public virtual DbSet<WorkOrderService> WorkOrderServices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseNpgsql("Host=localhost;Database=autocom_db;Username=postgres;Password=2612467");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("clients_pkey");

            entity.HasOne(d => d.User).WithOne(p => p.Client)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("clients_user_id_fkey");
        });

        modelBuilder.Entity<Mechanic>(entity =>
        {
            entity.HasKey(e => e.MechanicId).HasName("mechanics_pkey");

            entity.Property(e => e.MechanicId).HasDefaultValueSql("nextval('mechanics_id_mechanic_seq'::regclass)");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServicesId).HasName("services_pkey");
        });

        modelBuilder.Entity<SparePart>(entity =>
        {
            entity.HasKey(e => e.SparePartsId).HasName("spare_parts_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("vehicles_pkey");

            entity.HasOne(d => d.Client).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("vehicles_client_id_fkey");
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasKey(e => e.WorkOrderId).HasName("work_order_pkey");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValueSql("'ClientRequest'::character varying");

            entity.HasOne(d => d.Client).WithMany(p => p.WorkOrders)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("work_order_client_id_fkey");

            entity.HasOne(d => d.Mechanic).WithMany(p => p.WorkOrders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("work_order_mechanic_id_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.WorkOrders)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("work_order_vehicle_id_fkey");
        });

        modelBuilder.Entity<WorkOrderPart>(entity =>
        {
            entity.HasKey(e => new { e.WorkOrderId, e.SparePartsId }).HasName("work_order_parts_pkey");

            entity.HasOne(d => d.SpareParts).WithMany(p => p.WorkOrderParts)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("work_order_parts_spare_parts_id_fkey");

            entity.HasOne(d => d.WorkOrder).WithMany(p => p.WorkOrderParts).HasConstraintName("work_order_parts_work_order_id_fkey");
        });

        modelBuilder.Entity<WorkOrderService>(entity =>
        {
            entity.HasKey(e => new { e.WorkOrderId, e.ServicesId }).HasName("work_order_services_pkey");

            entity.HasOne(d => d.Services).WithMany(p => p.WorkOrderServices)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("work_order_services_services_id_fkey");

            entity.HasOne(d => d.WorkOrder).WithMany(p => p.WorkOrderServices).HasConstraintName("work_order_services_work_order_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
