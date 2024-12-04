using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RockyPixels.Models;

public partial class RockyPixelsBlogContext : DbContext
{
    public RockyPixelsBlogContext()
    {
    }

    public RockyPixelsBlogContext(DbContextOptions<RockyPixelsBlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostTag> PostTags { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=RockyPixelsBlog;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_Categories_1");

            entity.ToTable("Categories", "RockyPixels");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments", "RockyPixels");

            entity.Property(e => e.Author)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CommentContent)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.ParentPost).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ParentPostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comments_Posts");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK_Images_1");

            entity.ToTable("Images", "RockyPixels");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126038F22F0577");

            entity.ToTable("Posts", "RockyPixels");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Author)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PostContent)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Topic)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Posts_Categories");

            entity.HasOne(d => d.Image).WithMany(p => p.Posts)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK_Posts_Images");
        });

        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.TagId });

            entity.ToTable("PostTags", "RockyPixels");

            entity.Property(e => e.AlternateKey).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Post).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostTags_Posts");

            entity.HasOne(d => d.Tag).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostTags_Tags");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags", "RockyPixels");

            entity.Property(e => e.TagName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
