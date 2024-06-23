using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models;

public partial class KycbizContext : DbContext
{
    public KycbizContext()
    {
    }

    public KycbizContext(DbContextOptions<KycbizContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BusinessInfo> BusinessInfos { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EmailLog> EmailLogs { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<ForgotPasswordLog> ForgotPasswordLogs { get; set; }

    public virtual DbSet<KycDocType> KycDocTypes { get; set; }

    public virtual DbSet<KycInfo> KycInfos { get; set; }

    public virtual DbSet<KycInfoDoc> KycInfoDocs { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<MessageContentMaster> MessageContentMasters { get; set; }

    public virtual DbSet<OtpLog> OtpLogs { get; set; }

    public virtual DbSet<PasswordHistory> PasswordHistories { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    public virtual DbSet<UserLog> UserLogs { get; set; }

    public virtual DbSet<UserMerchantLog> UserMerchantLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessInfo>(entity =>
        {
            entity.ToTable("BusinessInfo");

            entity.Property(e => e.BusinessInfoId).HasColumnName("BusinessInfoID");
            entity.Property(e => e.BusinessContactNumber).HasMaxLength(20);
            entity.Property(e => e.BusinessName).HasMaxLength(150);
            entity.Property(e => e.BusinessType).HasMaxLength(30);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LocationName).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.ZipCode).HasMaxLength(25);

            entity.HasOne(d => d.City).WithMany(p => p.BusinessInfos)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_BusinessInfo_City");

            entity.HasOne(d => d.Country).WithMany(p => p.BusinessInfos)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_BusinessInfo_Country");

            entity.HasOne(d => d.State).WithMany(p => p.BusinessInfos)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK_BusinessInfo_State");

            entity.HasOne(d => d.User).WithMany(p => p.BusinessInfos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_BusinessInfo_UserInfo");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CityName).HasMaxLength(120);
            entity.Property(e => e.StateId).HasColumnName("StateID");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");

            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CountryName).HasMaxLength(150);
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.EmailLogSk);

            entity.ToTable("EMailLog");

            entity.Property(e => e.EmailLogSk).HasColumnName("EMailLog_Sk");
            entity.Property(e => e.Cc).HasColumnName("CC");
            entity.Property(e => e.ContentType).HasMaxLength(150);
            entity.Property(e => e.SendOn).HasColumnType("datetime");
            entity.Property(e => e.UserSk).HasColumnType("numeric(18, 0)");
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorId);

            entity.ToTable("ErrorLog");

            entity.Property(e => e.ErrorId)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Error_Id");
            entity.Property(e => e.ErrorDate).HasColumnType("datetime");
            entity.Property(e => e.ErrorMsg).IsUnicode(false);
            entity.Property(e => e.FunctionName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("IPaddress");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ModuleId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Module_Id");
            entity.Property(e => e.PageName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("User_Id");
        });

        modelBuilder.Entity<ForgotPasswordLog>(entity =>
        {
            entity.HasKey(e => e.ForgotPasswordid);

            entity.ToTable("ForgotPassword_Log");

            entity.Property(e => e.IsTokenVerified).HasColumnName("isTokenVerified");
            entity.Property(e => e.LastPasswordResetRequest).HasColumnType("datetime");
            entity.Property(e => e.TokenExpired).HasColumnType("datetime");
            entity.Property(e => e.TokenIssued).HasColumnType("datetime");
        });

        modelBuilder.Entity<KycDocType>(entity =>
        {
            entity.HasKey(e => e.KycDocId);

            entity.ToTable("KycDocType");

            entity.Property(e => e.KycDocId).HasColumnName("KycDocID");
            entity.Property(e => e.KycDocName).HasMaxLength(50);
        });

        modelBuilder.Entity<KycInfo>(entity =>
        {
            entity.ToTable("KycInfo");

            entity.HasIndex(e => e.KycInfoId, "IX_KycInfo");

            entity.Property(e => e.KycInfoId).HasColumnName("KycInfoID");
            entity.Property(e => e.BusinessInfoId).HasColumnName("BusinessInfoID");
            entity.Property(e => e.KycDocTypeId).HasColumnName("KycDocTypeID");
            entity.Property(e => e.KycDocUrl)
                .HasMaxLength(300)
                .HasColumnName("KycDocURL");
            entity.Property(e => e.KycExpireDate).HasColumnType("datetime");
            entity.Property(e => e.KycNumber).HasMaxLength(30);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.BusinessInfo).WithMany(p => p.KycInfos)
                .HasForeignKey(d => d.BusinessInfoId)
                .HasConstraintName("FK_KycInfo_BusinessInfo");

            entity.HasOne(d => d.KycDocType).WithMany(p => p.KycInfos)
                .HasForeignKey(d => d.KycDocTypeId)
                .HasConstraintName("FK_KycInfo_KycDocType");

            entity.HasOne(d => d.User).WithMany(p => p.KycInfos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_KycInfo_UserInfo");
        });

        modelBuilder.Entity<KycInfoDoc>(entity =>
        {
            entity.ToTable("KycInfoDoc");

            entity.Property(e => e.KycInfoDocId).HasColumnName("KycInfoDocID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DocFileExn)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.DocNameDesc).HasMaxLength(250);
            entity.Property(e => e.KycInfoId).HasColumnName("KycInfoID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.KycInfo).WithMany(p => p.KycInfoDocs)
                .HasForeignKey(d => d.KycInfoId)
                .HasConstraintName("FK_KycInfoDoc_KycInfo");

            entity.HasOne(d => d.User).WithMany(p => p.KycInfoDocs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_KycInfoDoc_UserInfo");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.Level).HasMaxLength(128);
        });

        modelBuilder.Entity<MessageContentMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MessageContentMaster");

            entity.Property(e => e.ContentType).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmailContent).HasColumnName("EMail_Content");
            entity.Property(e => e.MessageContentMasterSk)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("MessageContentMaster_Sk");
            entity.Property(e => e.ModifiedBy).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SmsContent).HasColumnName("SMS_Content");
        });

        modelBuilder.Entity<OtpLog>(entity =>
        {
            entity.HasKey(e => e.Otpid);

            entity.ToTable("OtpLog");

            entity.Property(e => e.OtpExpired).HasColumnType("datetime");
            entity.Property(e => e.OtpIssued).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.OtpLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_OtpLog_UserInfo");
        });

        modelBuilder.Entity<PasswordHistory>(entity =>
        {
            entity.ToTable("PasswordHistory");

            entity.Property(e => e.PasswordHistoryId).HasColumnName("PasswordHistory_id");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(200);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.StateName).HasMaxLength(120);
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserInfo");

            entity.Property(e => e.ContactNumber).HasMaxLength(30);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.ProfileFileType).HasMaxLength(100);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(300)
                .HasColumnName("ProfileURL");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<UserLog>(entity =>
        {
            entity.ToTable("UserLog");

            entity.Property(e => e.UserLogId).HasColumnName("UserLogID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogContent).HasMaxLength(250);
            entity.Property(e => e.Logtype)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserLog_UserInfo");
        });

        modelBuilder.Entity<UserMerchantLog>(entity =>
        {
            entity.ToTable("UserMerchantLog");

            entity.Property(e => e.UserMerchantLogId).HasColumnName("UserMerchantLogID");
            entity.Property(e => e.ConnectedStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MerchantUserId).HasColumnName("MerchantUserID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MerchantUser).WithMany(p => p.UserMerchantLogMerchantUsers)
                .HasForeignKey(d => d.MerchantUserId)
                .HasConstraintName("FK_UserMerchantLog_UserInfo1");

            entity.HasOne(d => d.User).WithMany(p => p.UserMerchantLogUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserMerchantLog_UserInfo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
