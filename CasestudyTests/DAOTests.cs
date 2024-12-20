using HelpdeskDAL;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CasestudyTests
{
    public class DAOTests
    {
        private readonly ITestOutputHelper output;
        public DAOTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmplyee = await dao.GetByEmail("bs@abc.com");
            Assert.NotNull(selectedEmplyee);
        }

        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetById(1);
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {

            EmployeeDAO dao = new();
            List<Employee> allEmployees = await dao.GetAll();
            Assert.True(allEmployees.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "Van",
                LastName = "Naranjo",
                PhoneNo = "(289)941-3872",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "v_naranjo@fanshaweonline.ca"
            };
            Assert.True(await dao.Add(newEmployee) > 0);

        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForUpdate = await dao.GetByEmail("v_naranjo@fanshaweonline.ca");
            if (employeeForUpdate != null)
            {
                string oldPhoneNo = employeeForUpdate.PhoneNo!;
                string newPhoneNo = oldPhoneNo == "(289)941-3872" ? "(555)555-5555" : "(289)941-3872";
                employeeForUpdate!.PhoneNo = newPhoneNo;
            }
            Assert.True(await dao.Update(employeeForUpdate!) == UpdateStatus.Ok); // 1 indicates the # of rows updated

        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForDelete = await dao.GetByEmail("v_naranjo@fanshaweonline.ca");
            if (employeeForDelete != null)
            {
                Assert.True(await dao.Delete(employeeForDelete.Id) == 1); // # of rows deleted
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetByPhoneNumber("(555) 555-5551");
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_ConcurrencyTest()
        {
            EmployeeDAO dao1 = new();
            EmployeeDAO dao2 = new();
            Employee employeeForUpdate1 = await dao1.GetByEmail("v_naranjo@fanshaweonline.ca");
            Employee employeeForUpdate2 = await dao2.GetByEmail("v_naranjo@fanshaweonline.ca");
            if (employeeForUpdate1 != null)
            {
                string? oldPhoneNo = employeeForUpdate1.PhoneNo;
                string? newPhoneNo = oldPhoneNo == "(289)941-3872" ? "555-555-5555" : "(289)941-3872";
                employeeForUpdate1.PhoneNo = newPhoneNo;
                if (await dao1.Update(employeeForUpdate1) == UpdateStatus.Ok)
                {
                    // need to change the phone # to something else
                    employeeForUpdate2.PhoneNo = "666-666-6668";
                    Assert.True(await dao2.Update(employeeForUpdate2) == UpdateStatus.Stale);
                }
                else
                    Assert.True(false); // first update failed
            }
            else
                Assert.True(false); // didn't find student 1
        }


        [Fact]
        public async Task Employee_LoadPicsTest()
        {
            {
                PicsUtility util = new();
                Assert.True(await util.AddEmployeePicsToDb());
            }
        }

        [Fact]
        public async Task Employee_ComprehensiveTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "Joe",
                LastName = "Smith",
                PhoneNo = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "js@abc.com"
            };
            int newEmployeeId = await dao.Add(newEmployee);
            output.WriteLine("New Employee Generated - Id = " + newEmployeeId);
            newEmployee = await dao.GetById(newEmployeeId);
            byte[] oldtimer = newEmployee.Timer!;
            output.WriteLine("New Employee " + newEmployee.Id + " Retrieved");
            newEmployee.PhoneNo = "(555)555-1233";
            if (await dao.Update(newEmployee) == UpdateStatus.Ok)
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was updated to - " + newEmployee.PhoneNo);
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was not updated!");
            }
            newEmployee.Timer = oldtimer; // to simulate another user
            newEmployee.PhoneNo = "doesn't matter data is stale now";
            if (await dao.Update(newEmployee) == UpdateStatus.Stale)
            {
                output.WriteLine("Employee " + newEmployeeId + " was not updated due to stale data");
            }

            dao = new();
            await dao.GetById(newEmployeeId);
            if (await dao.Delete(newEmployeeId) == 1)
            {
                output.WriteLine("Employee " + newEmployeeId + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetById(newEmployeeId));
        }

        [Fact]
        public async Task Call_ComprehensiveTest()
        {
            CallDAO dao = new();
            EmployeeDAO employeedao = new();
            ProblemDAO problemdao = new();

            Employee employees = await employeedao.GetByLastName("Naranjo");
            Employee techEmployees = await employeedao.GetByLastName("Burner");
            Problem badDrives = await problemdao.GetByDescription("Hard Drive Failure");

            Call newCall = new()
            {
                EmployeeId = employees.Id,
                TechId = techEmployees.Id,
                ProblemId = badDrives.Id,
                DateOpened = DateTime.Now,
                DateClosed = null,
                OpenStatus = true,
                Notes = "Naranjo drive is shot, Burner to fix it"
            };

            int newCallId = await dao.Add(newCall);
            output.WriteLine("New Call Generated - Id = " + newCallId);
            newCall = await dao.GetById(newCallId);
            byte[] oldtimer = newCall.Timer!;
            output.WriteLine("New Call " + newCall.Id + " Retrieved");
            string problemNote = newCall.Notes;
            newCall.Notes = "\n Ordered new drive!";
            if (await dao.Update(newCall) == UpdateStatus.Ok)
            {
                output.WriteLine("Call was updated " + problemNote + newCall.Notes);
            }
            else
            {
                output.WriteLine("Call " + newCallId + " phone# was not updated!");
            }
            newCall.Timer = oldtimer; // to simulate another user
            newCall.Notes = "doesn't matter data is stale now";
            if (await dao.Update(newCall) == UpdateStatus.Stale)
            {
                output.WriteLine("Call " + newCallId + " was not updated due to stale data");
            }

            dao = new();
            await dao.GetById(newCallId);
            if (await dao.Delete(newCallId) == 1)
            {
                output.WriteLine("Call " + newCallId + " was deleted!");
            }
            else
            {
                output.WriteLine("Call " + newCallId + " was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetById(newCallId));
        }



    }
}