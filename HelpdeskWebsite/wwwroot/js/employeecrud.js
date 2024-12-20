$(() => { // main jQuery routine - executes once on page load, where `$` is shorthand for jQuery.

    // Fetch all employee and department information from the server
    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding Employee Information...");

            // Request employee data from the server
            let response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json(); // Await the JSON parsing of the response
                buildEmployeeList(payload); // Populate the employee list on the UI
                msg === "" ?
                    $("#status").text("Employees Loaded") : $("#status").text(`${msg} - Employees Loaded`);
            } else if (response.status !== 404) { // Handle client-side errors
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // Handle 404 errors specifically
                $("#status").text("no such path on server");
            }

            // Request department data from the server
            response = await fetch(`api/department`);
            if (response.ok) {
                let divs = await response.json(); // Await the JSON parsing of the response
                sessionStorage.setItem("alldepartment", JSON.stringify(divs)); // Cache department data in sessionStorage
            } else if (response.status !== 404) { // Handle client-side errors
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // Handle 404 errors specifically
                $("#status").text("no such path on server");
            }
            }

        } catch (error) {
            $("#status").text(error.message); // Display any errors that occur during the fetch operations
        }
    }; // getAll

    // Generate the employee list on the UI
    const buildEmployeeList = (data) => {
        $("#employeeList").empty(); // Clear the existing list
        div = $(`<div class="list-group-item text-black bg-primary row d-flex" id="status">Employee Info</div>
            <div class="list-group-item row d-flex text-center text-primary" id="heading">
                <div class="col-2 h4">Title</div>
                <div class="col-4 h4">First</div>
                <div class="col-2 h4">Last</div>
                <div class="col-4 h4">Department</div>
            </div>`);
        div.appendTo($("#employeeList"));

        sessionStorage.setItem("allemployees", JSON.stringify(data)); // Cache employee data in sessionStorage
        btn = $(`<button class="list-group-item row d-flex" id="0">...click to add employee</button>`);
        btn.appendTo($("#employeeList"));

        // Generate a button for each employee with their details
        data.forEach(emp => {
            btn = $(`<button class="list-group-item row d-flex text-info" id="${emp.id}">`);
            btn.html(`
                <div class="col-2" id="employeetitle${emp.id}">${emp.title}</div>
                <div class="col-4" id="employeefname${emp.id}">${emp.firstname}</div>
                <div class="col-2" id="employeeelastnam${emp.id}">${emp.lastname}</div>
                <div class="col-4" id="ddlDepartment${emp.id}">${emp.departmentName}</div>
            `);
            btn.appendTo($("#employeeList"));
        });
    }; // buildEmployeeList

    // Initialize by fetching data from the server
    getAll("");

    // Event listener for clicks on the employee list to handle both update and add
    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allemployees"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data); // Determine if adding or updating
        } else {
            return false; // Ignore clicks on heading or status
        }
    });

    // Function to update an employee's data on the server
    const update = async (e) => {
        try {
            let emp = JSON.parse(sessionStorage.getItem("employee")); // Retrieve the employee object from sessionStorage
            // Populate employee properties from the input fields
            emp.title = $("#TextBoxTitle").val();
            emp.firstname = $("#TextBoxFirst").val();
            emp.lastname = $("#TextBoxLast").val();
            emp.email = $("#TextBoxEmail").val();
            emp.phoneno = $("#TextBoxPhone").val();
            emp.departmentId = parseInt($("#ddlDepartment").val());

            // Send the updated employee data to the server using a PUT request
            let response = await fetch("api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp),
            });
            if (response.ok) {
                let payload = await response.json();
                $("#status").text(payload.msg); // Display a success message
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        }
    };

    // Error handling function for server responses
    const errorRtn = (problemJson, status) => {
        if (status > 499) {
            $("#status").text("Problem server side, see debug console");
        } else {
            let keys = Object.keys(problemJson.errors);
            problem = {
                status: status,
                statusText: problemJson.errors[keys[0]][0],
            };
            $("#status").text("Problem client side, see browser console");
            console.log(problem);
        }
    }

    // Clear input fields in the modal for a fresh input
    const clearModalFields = () => {
        loadDepartmentDDL(-1);
        $("#TextBoxTitle").val("");
        $("#TextBoxFirst").val("");
        $("#TextBoxLast").val("");
        $("#TextBoxEmail").val("");
        $("#TextBoxPhone").val("");
        sessionStorage.removeItem("employee"); // Clear any previously stored employee data
        $("#theModal").modal("toggle"); // Toggle modal visibility
    };

    // Configure modal for adding a new employee
    const setupForAdd = () => {
        $("#deletebutton").hide();
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>add employee</h4>");
        $("#theModal").modal("toggle");
        $("#modalstatus").text("add new employee");
        $("#theModalLabel").text("Add");
        clearModalFields();
    };

    // Configure modal for updating existing employee data
    const setupForUpdate = (id, data) => {
        $("#deletebutton").show();
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>update employee</h4>");
        clearModalFields();
        data.forEach(employee => {
            if (employee.id === parseInt(id)) {
                // Populate modal fields with the employee's data
                $("#TextBoxTitle").val(employee.title);
                $("#TextBoxFirst").val(employee.firstname);
                $("#TextBoxLast").val(employee.lastname);
                $("#TextBoxEmail").val(employee.email);
                $("#TextBoxPhone").val(employee.phoneno);

                sessionStorage.setItem("employee", JSON.stringify(employee)); // Cache employee data
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle");
                $("#theModalLabel").text("Update");
                loadDepartmentDDL(employee.departmentId);
            }
        });
    };

    // Add a new employee to the server
    const add = async () => {
        try {
            let emp = {}; // Create a new employee object
            // Populate employee properties from the input fields
            emp.title = $("#TextBoxTitle").val();
            emp.firstname = $("#TextBoxFirst").val();
            emp.lastname = $("#TextBoxLast").val();
            emp.email = $("#TextBoxEmail").val();
            emp.phoneno = $("#TextBoxPhone").val();
            emp.DepartmentId = parseInt($("#ddlDepartment").val());
            emp.id = -1;
            emp.timer = null;
            emp.picture64 = null;

            // Send the employee data to the server using a POST request
            let response = await fetch("api/employee", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp),
            });
            if (response.ok) {
                let data = await response.json();
                getAll(data.msg); // Refresh the list with the new employee
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }
        } catch (error) {
            $("#status").text(error.message);
        }
        $("#theModal").modal("toggle"); // Close the modal
    };

    // Trigger add or update based on the button's value
    $("#actionbutton").on("click", () => {
        $("#actionbutton").val() === "update" ? update() : add();
    });

    // Delete an employee from the server
    const _delete = async () => {
        let employee = JSON.parse(sessionStorage.getItem("employee"));
        try {
            let response = await fetch(`api/employee/${employee.id}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json; charset=utf-8' }
            });
            if (response.ok) {
                let data = await response.json();
                getAll(data.msg); // Refresh the list after deletion
            } else {
                $('#status').text(`Status - ${response.status}, Problem on delete server side, see server console`);
            }
            $('#theModal').modal('toggle'); // Close the modal
        } catch (error) {
            $('#status').text(error.message);
        }
    };

    // Configure delete confirmation dialog visibility
    $("#dialog").hide();
    $("#deletebutton").on("click", (e) => {
        $("#dialog").show();
        $("#status").text("");
        $("#dialogbutton").hide();
    });
    $("#nobutton").on("click", (e) => {
        $("#dialog").hide();
        $("#dialogbutton").show();
    });
    $("#yesbutton").on("click", () => {
        $("#dialog").hide();
        _delete();
    });

    // Populate department dropdown list with data from session storage
    const loadDepartmentDDL = (empdiv) => {
        let html = '';
        $('#ddlDepartment').empty();
        let alldepartment = JSON.parse(sessionStorage.getItem('alldepartment'));
        alldepartment.forEach((div) => { html += `<option value="${div.id}">${div.departmentName}</option>` });
        $('#ddlDepartment').append(html);
        $('#ddlDepartment').val(empdiv); // Set dropdown to the employee's department if applicable
    };

}); // jQuery ready method
