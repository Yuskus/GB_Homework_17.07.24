using Microsoft.EntityFrameworkCore;

namespace HomeworkGB9.Model
{
    public partial class ChatDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies().UseNpgsql("Host=localhost;Username=postgres;Password=22011995;Database=ChatDb");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id)
                      .HasName("users_pkey");

                entity.ToTable("users");

                entity.Property(u => u.Id)
                      .HasColumnName("id");
                entity.Property(u => u.Name)
                      .HasMaxLength(255)
                      .HasColumnName("name");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id)
                      .HasName("messages_pkey");

                entity.ToTable("messages");

                entity.Property(m => m.Id)
                      .HasColumnName("id");
                entity.Property(m => m.Text)
                      .HasColumnName("text");
                entity.Property(m => m.CreationTime)
                      .HasColumnType("timestamp")
                      .HasColumnName("creation_time");
                entity.Property(m => m.IsReceived)
                      .HasColumnName("is_recieved");
                entity.Property(m => m.SenderId)
                      .HasColumnName("sender_id");
                entity.Property(m => m.RecipientId)
                      .HasColumnName("recipient_id");

                entity.HasOne(m => m.Sender)
                      .WithMany(u => u.FromMessages)
                      .HasForeignKey(m => m.SenderId)
                      .HasConstraintName("messages_from_user_id_fk");
                entity.HasOne(m => m.Recipient)
                      .WithMany(u => u.ToMessages)
                      .HasForeignKey(m => m.RecipientId)
                      .HasConstraintName("messages_to_user_id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
