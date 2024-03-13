I'm setting an Id Strategy on a document type in [Program](MartenIssues/Program.cs#L6).

Using [IDocumentStore.BulkInsertAsync](MartenIssues/Program.cs#L18) does not trigger the id generation.

Using [IDocumentSession.SaveChangesAsync](MartenIssues/Program.cs#L26) triggers the id generation as expected.
