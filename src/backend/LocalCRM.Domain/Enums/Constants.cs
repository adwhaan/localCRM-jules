namespace LocalCRM.Domain.Enums
{
    public static class ActionTypes
    {
        public const string Create = "CREATE";
        public const string Update = "UPDATE";
        public const string Delete = "DELETE";
        public const string SoftDelete = "SOFT_DELETE";
        public const string Restore = "RESTORE";
        public const string LinkAdd = "LINK_ADD";
        public const string LinkRemove = "LINK_REMOVE";
        public const string Error = "ERROR";
    }

    public static class Permissions
    {
        public const string CompaniesRead = "companies.read";
        public const string CompaniesReadDeleted = "companies.read_deleted";
        public const string CompaniesCreate = "companies.create";
        public const string CompaniesUpdate = "companies.update";
        public const string CompaniesDelete = "companies.delete";
        public const string CompaniesRestore = "companies.restore";

        public const string ContactsRead = "contacts.read";
        public const string ContactsReadDeleted = "contacts.read_deleted";
        public const string ContactsCreate = "contacts.create";
        public const string ContactsUpdate = "contacts.update";
        public const string ContactsDelete = "contacts.delete";
        public const string ContactsRestore = "contacts.restore";

        public const string InteractionsRead = "interactions.read";
        public const string InteractionsReadDeleted = "interactions.read_deleted";
        public const string InteractionsCreate = "interactions.create";
        public const string InteractionsUpdate = "interactions.update";
        public const string InteractionsDelete = "interactions.delete";
        public const string InteractionsRestore = "interactions.restore";

        public const string EngagementsRead = "engagements.read";
        public const string EngagementsReadDeleted = "engagements.read_deleted";
        public const string EngagementsCreate = "engagements.create";
        public const string EngagementsUpdate = "engagements.update";
        public const string EngagementsDelete = "engagements.delete";
        public const string EngagementsRestore = "engagements.restore";

        public const string NotesRead = "notes.read";
        public const string NotesReadDeleted = "notes.read_deleted";
        public const string NotesCreate = "notes.create";
        public const string NotesUpdate = "notes.update";
        public const string NotesDelete = "notes.delete";
        public const string NotesRestore = "notes.restore";

        public const string DocumentsRead = "documents.read";
        public const string DocumentsReadDeleted = "documents.read_deleted";
        public const string DocumentsCreate = "documents.create";
        public const string DocumentsUpdate = "documents.update";
        public const string DocumentsDelete = "documents.delete";
        public const string DocumentsRestore = "documents.restore";

        public const string UsersRead = "users.read";
        public const string UsersCreate = "users.create";
        public const string UsersUpdate = "users.update";
        public const string UsersDisable = "users.disable";
        public const string UsersManage = "users.manage";

        public const string RolesAssignPermissions = "roles.assign_permissions";
        public const string SettingsManage = "settings.manage";
        public const string AuditRead = "audit.read";
        public const string DocVisibilityElevate = "doc_visibility.elevate";
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
    }
}
