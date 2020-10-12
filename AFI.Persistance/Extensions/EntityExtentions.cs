using AFI.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AFI.Persistance.Extensions
{
    public static class EntityExtentions
    {
        public static ModelBuilder ConfigureBaseEntity<TEntity>(this ModelBuilder buildAction, string tableName, string schemaName) where TEntity : BaseEntity
        {
            return buildAction.Entity<TEntity>(entity =>
            {
                entity.ToTable(tableName, schemaName);
                entity.HasIndex(e => e.Id);

                entity.Property(e => e.CreatedByUserId).IsRequired();

                entity.Property(e => e.CreatedUTC)
                .IsRequired()
                .HasDefaultValueSql("GetUtcDate()");

                entity.Property(e => e.EditedUTC);

                entity.Property(e => e.LastEditedByUserId);
            });
        }
    }
}
