using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{
    [Table("Users")]
    public class BeautyBotUserModel
    {
        [Column("Id"), PrimaryKey]
        public Guid UserId { get; set; }

        [Column("TelegramId"), NotNull]
        public long TelegramUserId { get; set; }

        [Column("UserName"), NotNull]
        public string? TelegramUserName { get; set; }

        [Column("FirstName"), NotNull]
        public string? TelegramUserFirstName { get; set; }

        [Column("LastName"), NotNull]
        public string? TelegramUserLastName { get; set; }

        [Column("RegisteredAt"), NotNull]
        public DateTime RegisteredAt { get; set; }
    }
}


//CREATE TABLE "Appointments" (
//    "Id" UUID PRIMARY KEY,
//	"UserId" UUID NOT NULL,

	
//    "TelegramId" BIGINT NOT NULL UNIQUE,
//    "UserName" VARCHAR(100),
//    "FirstName" VARCHAR(100),
//    "LastName" VARCHAR(100),
//    "RegisteredAt" TIMESTAMP NOT NULL

//	FOREIGN KEY ("UserId") REFERENCES "Users"("Id")
//);

//[Table("Appointments")]
//public class AppointmentModel
//{
//    [Column("Id"), PrimaryKey]
//    public Guid Id { get; set; }

//    [Column("UserId"), PrimaryKey]
//    public Guid UserId { get; set; }

//    [Association(ThisKey = nameof(UserId), OtherKey = nameof(BeautyBotUserModel.UserId))]
//    public BeautyBotUserModel User { get; set; }

//    [Column("Type"), NotNull]
//    public Procedure Type { get; set; }

//    [Column("Subtype"), NotNull]
//    public required string Subtype { get; set; }

//    [Column("Price"), NotNull]
//    public decimal Price { get; set; }

//    [Column("Duration"), NotNull]
//    public int Duration { get; set; }
//}
//CREATE INDEX ON "Appointments" ("Id");

//public class Appointment
//{
//    public Guid Id { get; set; }
//    public BeautyBotUser User { get; set; }
//    public IProcedure Procedure { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public DateTime StateChangedAt { get; set; }
//    public DateTime AppointmentDate { get; set; }
//    public int AppointmentDuration { get; set; }
//    public AppointmentState State { get; set; }

//    public Appointment(BeautyBotUser user, IProcedure procedure, DateOnly appointmentDate, TimeOnly appointmentTime)
//    {
//        Id = Guid.NewGuid();
//        User = user;
//        Procedure = procedure;
//        CreatedAt = DateTime.Now;
//        StateChangedAt = DateTime.Now;
//        AppointmentDate = appointmentDate.ToDateTime(appointmentTime);
//        AppointmentDuration = procedure.Duration;
//        State = AppointmentState.Active;
//    }
//}









//CREATE TABLE "Procedures" (
//    "Id" UUID PRIMARY KEY,
//    "Type" VARCHAR(100) NOT NULL CHECK ("Type" <> ''),
//    "Subtype" VARCHAR(100) NOT NULL CHECK ("Subtype" <> ''),
//    "Price" DECIMAL(10,2) NOT NULL,
//    "Duration" INT NOT NULL
//);

//CREATE TABLE "Users" (
//    "Id" UUID PRIMARY KEY,
//    "TelegramId" BIGINT NOT NULL UNIQUE,
//    "UserName" VARCHAR(100),
//    "FirstName" VARCHAR(100),
//    "LastName" VARCHAR(100),
//    "RegisteredAt" TIMESTAMP NOT NULL
//);

//CREATE INDEX ON "Users" ("TelegramId");