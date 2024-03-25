using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Go2Hotel.Models
{
    public partial class Booking_HomestayContext : DbContext
    {
        public Booking_HomestayContext()
        {
        }

        public Booking_HomestayContext(DbContextOptions<Booking_HomestayContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<AdminTransaction> AdminTransactions { get; set; } = null!;
        public virtual DbSet<AdminWallet> AdminWallets { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Guest> Guests { get; set; } = null!;
        public virtual DbSet<GuestTransaction> GuestTransactions { get; set; } = null!;
        public virtual DbSet<GuestWallet> GuestWallets { get; set; } = null!;
        public virtual DbSet<Homestay> Homestays { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Owner> Owners { get; set; } = null!;
        public virtual DbSet<OwnerTransaction> OwnerTransactions { get; set; } = null!;
        public virtual DbSet<OwnerWallet> OwnerWallets { get; set; } = null!;
        public virtual DbSet<Rate> Rates { get; set; } = null!;
        public virtual DbSet<TypeHomestay> TypeHomestays { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =(local); database = Booking_Homestay; uid=sa;pwd=2062002;Trusted_Connection=True;Encrypt=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.AdminEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("admin_email");

                entity.Property(e => e.AdminPassword)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("admin_password");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshTokenExpireTime).HasColumnType("datetime");

                entity.Property(e => e.ResetPasswordToken).IsUnicode(false);
            });

            modelBuilder.Entity<AdminTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Admin_Tr__85C600AF323B6A1B");

                entity.ToTable("Admin_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.TransactionAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transaction_amount");

                entity.Property(e => e.TransactionContent)
                    .IsUnicode(false)
                    .HasColumnName("transaction_content");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_date");

                entity.Property(e => e.TransactionRemain)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transaction_remain");

                entity.Property(e => e.TransactionStatus).HasColumnName("transaction_status");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");
            });

            modelBuilder.Entity<AdminWallet>(entity =>
            {
                entity.HasKey(e => e.WalletId)
                    .HasName("PK__Admin_Wa__0EE6F04182CE044D");

                entity.ToTable("Admin_Wallet");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.WalletAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("wallet_amount");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.BookingDate)
                    .HasColumnType("datetime")
                    .HasColumnName("booking_date");

                entity.Property(e => e.BookingFrom)
                    .HasColumnType("datetime")
                    .HasColumnName("booking_from");

                entity.Property(e => e.BookingPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("booking_phone");

                entity.Property(e => e.BookingTo)
                    .HasColumnType("datetime")
                    .HasColumnName("booking_to");

                entity.Property(e => e.CheckinTime)
                    .HasColumnType("datetime")
                    .HasColumnName("checkin_time");

                entity.Property(e => e.CheckoutTime)
                    .HasColumnType("datetime")
                    .HasColumnName("checkout_time");

                entity.Property(e => e.GuestId).HasColumnName("guest_id");

                entity.Property(e => e.HomstayId).HasColumnName("homstay_id");

                entity.Property(e => e.TotalCost)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("total_cost");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.CommentContext).HasColumnName("comment_context");

                entity.Property(e => e.CommentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("comment_date");

                entity.Property(e => e.GuestId).HasColumnName("guest_id");

                entity.Property(e => e.HomestayId).HasColumnName("homestay_id");
            });

            modelBuilder.Entity<Guest>(entity =>
            {
                entity.ToTable("Guest");

                entity.Property(e => e.GuestId).HasColumnName("guest_id");

                entity.Property(e => e.GuestAddress)
                    .HasMaxLength(200)
                    .HasColumnName("guest_address");

                entity.Property(e => e.GuestAvatar)
                    .IsUnicode(false)
                    .HasColumnName("guest_avatar");

                entity.Property(e => e.GuestCccd)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("guest_cccd");

                entity.Property(e => e.GuestEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("guest_email");

                entity.Property(e => e.GuestName)
                    .HasMaxLength(50)
                    .HasColumnName("guest_name");

                entity.Property(e => e.GuestPassword)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("guest_password");

                entity.Property(e => e.GuestPhone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("guest_phone");

                entity.Property(e => e.GuestStatus).HasColumnName("guest_status");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshTokenExpireTime).HasColumnType("datetime");

                entity.Property(e => e.ResetPasswordToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GuestTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Guest_Tr__85C600AF68894D80");

                entity.ToTable("Guest_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.TransactionAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transaction_amount");

                entity.Property(e => e.TransactionContent)
                    .IsUnicode(false)
                    .HasColumnName("transaction_content");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_date");

                entity.Property(e => e.TransactionRemain)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transaction_remain");

                entity.Property(e => e.TransactionStatus).HasColumnName("transaction_status");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");
            });

            modelBuilder.Entity<GuestWallet>(entity =>
            {
                entity.HasKey(e => e.WalletId)
                    .HasName("PK__Guest_Wa__0EE6F0414428F61B");

                entity.ToTable("Guest_Wallet");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.Property(e => e.GuestId).HasColumnName("guest_id");

                entity.Property(e => e.WalletAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("wallet_amount");
            });

            modelBuilder.Entity<Homestay>(entity =>
            {
                entity.ToTable("Homestay");

                entity.Property(e => e.HomestayId).HasColumnName("homestay_id");

                entity.Property(e => e.HomestayBedroom).HasColumnName("homestay_bedroom");

                entity.Property(e => e.HomestayCity)
                    .HasMaxLength(20)
                    .HasColumnName("homestay_city");

                entity.Property(e => e.HomestayCountry)
                    .HasMaxLength(20)
                    .HasColumnName("homestay_country");

                entity.Property(e => e.HomestayDescription).HasColumnName("homestay_description");

                entity.Property(e => e.HomestayName).HasColumnName("homestay_name");

                entity.Property(e => e.HomestayPrice)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("homestay_price");

                entity.Property(e => e.HomestayRegion)
                    .HasMaxLength(20)
                    .HasColumnName("homestay_region");

                entity.Property(e => e.HomestaySodo).HasColumnName("homestay_sodo");

                entity.Property(e => e.HomestayStatus).HasColumnName("homestay_status");

                entity.Property(e => e.HomestayStreet)
                    .HasMaxLength(100)
                    .HasColumnName("homestay_street");

                entity.Property(e => e.HomestayType).HasColumnName("homestay_type");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.HomestayId).HasColumnName("homestay_id");

                entity.Property(e => e.Image1)
                    .IsUnicode(false)
                    .HasColumnName("image");
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.ToTable("Owner");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.OwnerAddress)
                    .HasMaxLength(200)
                    .HasColumnName("owner_address");

                entity.Property(e => e.OwnerAddress2)
                    .HasMaxLength(200)
                    .HasColumnName("owner_address_2");

                entity.Property(e => e.OwnerAvatar)
                    .IsUnicode(false)
                    .HasColumnName("owner_avatar");

                entity.Property(e => e.OwnerCccd)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("owner_cccd");

                entity.Property(e => e.OwnerCity)
                    .HasMaxLength(20)
                    .HasColumnName("owner_city");

                entity.Property(e => e.OwnerCredit)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("owner_credit");

                entity.Property(e => e.OwnerEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("owner_email");

                entity.Property(e => e.OwnerName)
                    .HasMaxLength(50)
                    .HasColumnName("owner_name");

                entity.Property(e => e.OwnerPassword)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("owner_password");

                entity.Property(e => e.OwnerPhone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("owner_phone");

                entity.Property(e => e.OwnerStatus).HasColumnName("owner_status");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshTokenExpireTime).HasColumnType("datetime");

                entity.Property(e => e.ResetPasswordToken).IsUnicode(false);
            });

            modelBuilder.Entity<OwnerTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Owner_Tr__85C600AF27062C8F");

                entity.ToTable("Owner_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.TransactionAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transaction_amount");

                entity.Property(e => e.TransactionContent)
                    .IsUnicode(false)
                    .HasColumnName("transaction_content");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_date");

                entity.Property(e => e.TransactionRemain)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transaction_remain");

                entity.Property(e => e.TransactionStatus).HasColumnName("transaction_status");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");
            });

            modelBuilder.Entity<OwnerWallet>(entity =>
            {
                entity.HasKey(e => e.WalletId)
                    .HasName("PK__Owner_Wa__0EE6F0415A51BA2A");

                entity.ToTable("Owner_Wallet");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.WalletAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("wallet_amount");
            });

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.ToTable("Rate");

                entity.Property(e => e.RateId).HasColumnName("rate_id");

                entity.Property(e => e.GuestId).HasColumnName("guest_id");

                entity.Property(e => e.HomestayId).HasColumnName("homestay_id");

                entity.Property(e => e.RateAmount).HasColumnName("rate_amount");
            });

            modelBuilder.Entity<TypeHomestay>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__Type_Hom__2C0005983D98F2C1");

                entity.ToTable("Type_Homestay");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .HasColumnName("type_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
