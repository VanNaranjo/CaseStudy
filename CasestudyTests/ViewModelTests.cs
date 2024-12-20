using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;
using HelpdeskViewModels;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace CasestudyTests
{
    public class ViewModelTests
    {
        private readonly ITestOutputHelper output;
        public ViewModelTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeViewModel vm = new() { Email = "v_naranjo@fanshaweonline.ca" };
            await vm.GetByEmail();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeViewModel vm = new() { Id = 1006 };
            await vm.GetById(1006);
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            List<EmployeeViewModel> allEmployeeVms;
            EmployeeViewModel vm = new();
            allEmployeeVms = await vm.GetAll();
            Assert.True(allEmployeeVms.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeViewModel vm;
            vm = new()
            {
                Title = "Mr.",
                Firstname = "Van",
                Lastname = "Naranjo",
                Email = "v_naranjo@fanshaweonline.ca",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure division id is in Division table
            };
            await vm.Add();
            Assert.True(vm.Id > 0);

        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777)777-7777" };
            await vm.GetByPhoneNumber(); // Employee just added in Add test
            vm.Phoneno = vm.Phoneno == "(777)777-7777" ? "(555)555-5552" : "(777)777-7777";
            // will be -1 if failed 0 if no data changed, 1 if succcessful
            Assert.True(await vm.Update() == 1);

        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeViewModel vm = new() { Email = "v_naranjo@fanshaweonline.ca" };
            await vm.GetByEmail(); // Employee just added
            Assert.True(await vm.Delete() == 1); // 1 student deleted


        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777)777-7777" };
            await vm.GetByPhoneNumber();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Student_ConcurrencyTest()
        {
            EmployeeViewModel vm1 = new() { Lastname = "v_naranjo@fanshaweonline.ca" };
            EmployeeViewModel vm2 = new() { Lastname = "v_naranjo@fanshaweonline.ca" };
            await vm1.GetByEmail(); // Fetch same student to simulate 2 users
            if (vm1.Lastname != "Not Found") // make sure we found a student
            {
                await vm2.GetByEmail(); // fetch same data
                vm1.Phoneno = vm1.Phoneno == "(289)941-3872" ? "(555)555-5552" : "(289)941-3872";
                if (await vm1.Update() == 1)
                {
                    vm2.Phoneno = "(666)666-6666"; // just need any value
                    Assert.True(await vm2.Update() == -2);
                }
            }
            else
            {
                Assert.True(false); // student not found
            }
        }

        [Fact]
        public async Task Employee_ComprehensiveVMTest()
        {
            EmployeeViewModel evm = new()
            {
                Title = "Mr.",
                Firstname = "Some",
                Lastname = "Employee",
                Email = "some@abc.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure department id is in Departments table
            };
            await evm.Add();
            output.WriteLine("New Employee Added - Id = " + evm.Id);
            int? id = evm.Id; // need id for delete later
            await evm.GetById((int)id!);    
            output.WriteLine("New Employee " + id + " Retrieved");
            evm.Phoneno = "(555)555-1233";
            if (await evm.Update() == 1)
            {
                output.WriteLine("Employee " + id + " phone# was updated to - " +
               evm.Phoneno);
            }
            else
            {
                output.WriteLine("Employee " + id + " phone# was not updated!");
            }
            evm.Phoneno = "Another change that should not work";
            if (await evm.Update() == -2)
            {
                output.WriteLine("Employee " + id + " was not updated due to stale data");
            }
            evm = new EmployeeViewModel
            {
                Id = id
            };
            // need to reset because of concurrency error
            await evm.GetById((int)id!);
            if (await evm.Delete() == 1)
            {
                output.WriteLine("Employee " + id + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + id + " was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
           => await evm.GetById((int)id!));
        }

        [Fact]
        public async Task Call_ComprehensiveVMTest()
        {
            EmployeeViewModel empVm = new();
            await empVm.GetByLastName("Naranjo");
            EmployeeViewModel techVm = new();
            await techVm.GetByLastName("Burner");
            ProblemViewModel probVm = new();
            await probVm.GetByDescription("Memory Upgrade");


            CallViewModel callVm = new()
            {
                EmployeeId = (int)empVm.Id!,
                TechId = (int)techVm.Id!,
                ProblemId = (int)probVm.Id!,
                DateOpened = DateTime.Now,
                DateClosed = null,
                OpenStatus = true,
                Notes = "Van has bad RAM, Burner to fix it"
            };
            await callVm.Add();
            output.WriteLine("New Call Added - Id = " + callVm.Id);
            int? id = callVm.Id; // need id for delete later
            callVm = new();
            await callVm.GetById((int)id!);
            output.WriteLine("New Call " + id + " Retrieved");
            callVm.Notes += "\n Ordered new RAM!";
            if (await callVm.Update() == 1)
            {
                output.WriteLine("Call was updated " + callVm.Notes);
            }
            else
            {
                output.WriteLine("Call " + id + " phone# was not updated!");
            }
            callVm.Notes = "Another change that should not work";
            if (await callVm.Update() == -2)
            {
                output.WriteLine("Call " + id + " was not updated due to stale data");
            }
            callVm = new CallViewModel
            {
                Id = (int)id
            };
            // need to reset because of concurrency error
            await callVm.GetById((int)id!);
            if (await callVm.Delete() == 1)
            {
                output.WriteLine("Call " + id + " was deleted!");
            }
            else
            {
                output.WriteLine("Call " + id + " was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
           => await callVm.GetById((int)id!));
        }


    }
}
