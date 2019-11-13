﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class BlogEntryRepository
    {
        private NorthwindContext _context;
        public BlogEntryRepository(NorthwindContext context)
        {
            _context = context;
        }
        public BlogEntry CreateBlogEntry(string title, string contents, string username)
        {
            var sql = string.Format("insert into BlogEntries (Title, Contents, Author, PostedDate) " +
                "values ('{0}','{1}','{2}','{3:yyyy-MM-dd}'); " +
                "select top 1 * from blogentries order by Id desc;",
                title, contents, username, DateTime.Now);

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            _context.Database.OpenConnection();
            var dataReader = command.ExecuteReader();
            var entry = new BlogEntry();
            while (dataReader.Read())
            {
                entry.Id = (int)dataReader[0];
                entry.Title = (string)dataReader[1];
                entry.Contents = (string)dataReader[2];
                entry.Author = (string)dataReader[3];
                entry.PostedDate = (DateTime)dataReader[4];
            }
            return entry;
        }
        public BlogEntry GetBlogEntry(int BlogEntryId)
        {
            return _context.BlogEntries.Single(b => b.Id == BlogEntryId);
        }
        public List<BlogEntry> GetTopBlogEntries()
        {
            return this.GetTopBlogEntries(4, 0);
        }
        public List<BlogEntry> GetTopBlogEntries(int NumberOfEntries)
        {
            return this.GetTopBlogEntries(NumberOfEntries, 0);
        }
        public List<BlogEntry> GetTopBlogEntries(int NumberOfEntries, int StartPosition)
        {
            var blogEntries = _context.BlogEntries
                .OrderByDescending(b => b.PostedDate)
                .Skip(StartPosition)
                .Take(NumberOfEntries);
            return blogEntries.ToList();
        }
    }
}
