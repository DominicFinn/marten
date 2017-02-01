﻿using System;
using Marten.Events;
using Marten.Util;

namespace Marten.Services.Events
{
    /// <summary>
    /// Raise exception inside transaction if expected maximum id for event stream
    /// does not match expectation
    /// </summary>
    public class AssertEventStreamMaxEventId : IStorageOperation
    {
        private readonly Guid stream;
        private readonly int expectedVersion;
        private readonly string tableName;

        public AssertEventStreamMaxEventId(Guid stream, int expectedVersion, string tableName)
        {
            this.stream = stream;
            this.expectedVersion = expectedVersion;
            this.tableName = tableName;
        }

        public void ConfigureCommand(CommandBuilder builder)
        {
            // Parameterized queries won't work here: https://github.com/npgsql/npgsql/issues/331
            // In order to parameterize, sproc would need to be created            
            var sql = $@"DO $$ BEGIN IF
  (SELECT max(events.version)
   FROM {tableName} AS events
   WHERE events.stream_id = '{stream}') <> {expectedVersion} THEN 
RAISE EXCEPTION '{EventContracts.UnexpectedMaxEventIdForStream.Value}'; END IF; END; $$;";

            
            builder.Append(sql);
        }

        public Type DocumentType => null;
    }
}