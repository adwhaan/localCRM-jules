namespace LocalCRM.WebApi.GraphQL
{
    public class MutationResult
    {
        public bool Success { get; set; }
        public int? Id { get; set; }

        public static MutationResult SuccessResult(int? id = null) => new() { Success = true, Id = id };
        public static MutationResult FailureResult() => new() { Success = false };
    }
}
