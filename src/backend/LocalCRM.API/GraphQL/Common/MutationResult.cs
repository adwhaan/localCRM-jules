namespace LocalCRM.API.GraphQL.Common;

public record MutationResult(bool Success, int? Id = null);
