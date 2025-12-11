using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using library_api.Domain.Entities;

namespace library_api.Infrastructure.Persistence.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.StudentName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(l => l.LoanDate)
                .IsRequired();

            builder.Property(l => l.ReturnDate)
                .IsRequired(false);

            builder.Property(l => l.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(l => l.CreatedAt)
                .IsRequired();

            builder.HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
