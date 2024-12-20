/*
Van Naranjo
December 9, 2024
This is the js file for employeecall.html that will do crud on the front-end
*/

$(() => { // main jQuery routine - executes once on page load, where `$` is shorthand for jQuery.

    // Fetch all call and problem information from the server
    const getAll = async (msg) => {
        try {
            $("#callList").text("Finding Call Information...");

            // Request call data from the server
            let response = await fetch(`api/call`);
            if (response.ok) {
                let payload = await response.json(); // Await the JSON parsing of the response
                buildCallList(payload); // Populate the call list on the UI
                msg === "" ?
                    $("#status").text("Calls Loaded") : $("#status").text(`${msg} - Calls Loaded`);
            } else if (response.status !== 404) { // Handle client-side errors
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // Handle 404 errors specifically
                $("#status").text("no such path on server");
            }

            // Request problem data from the server
            response = await fetch(`api/problem`);
            if (response.ok) {
                let divs = await response.json(); // Await the JSON parsing of the response
                sessionStorage.setItem("allproblem", JSON.stringify(divs)); // Cache problem data in sessionStorage
            } else if (response.status !== 404) { // Handle client-side errors
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // Handle 404 errors specifically
                $("#status").text("no such path on server");
            }

            // Request problem data from the server
            response = await fetch(`api/employee`);
            if (response.ok) {
                let divs = await response.json(); // Await the JSON parsing of the response
                sessionStorage.setItem("allemployee", JSON.stringify(divs)); // Cache problem data in sessionStorage
            } else if (response.status !== 404) { // Handle client-side errors
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // Handle 404 errors specifically
                $("#status").text("no such path on server");
            }

        } catch (error) {
            $("#status").text(error.message); // Display any errors that occur during the fetch operations
        }
    }; // getAll

    // Generate the call list on the UI
    const buildCallList = (data, usealldata = true) => {
        $("#callList").empty(); // Clear the existing list
        div = $(`<div class="list-group-item text-black bg-primary row d-flex" id="status">Call Info</div>
            <div class="list-group-item row d-flex text-center text-primary" id="heading">
                <div class="col-4 h4">Title</div>
                <div class="col-4 h4">First</div>
                <div class="col-4 h4">Last</div>
            </div>`);
        div.appendTo($("#callList"));

        usealldata ? sessionStorage.setItem("allcalls", JSON.stringify(data)) : null; // Cache call data in sessionStorage
        btn = $(`<button class="list-group-item row d-flex" id="0">...click to add call</button>`);
        btn.appendTo($("#callList"));


        // Generate a button for each call with their details
        data.forEach(emp => {
            btn = $(`<button class="list-group-item row d-flex text-info" id="${emp.id}">`);
            btn.html(`
                <div class="col-4" id="calltitle${emp.id}">${(emp.dateOpened).replace("T", " ")}</div>
                <div class="col-4" id="callfname${emp.id}">${emp.employeeName}</div>
                <div class="col-4" id="callelastnam${emp.id}">${emp.problemDescription}</div>
            `);
            btn.appendTo($("#callList"));
        });
    }; // buildCallList

    // Initialize by fetching data from the server
    getAll("");

    // Event listener for clicks on the call list to handle both update and add
    $("#callList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "callList" || id === "") {
            id = e.target.id;
        }
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allcalls"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data); // Determine if adding or updating
        } else {
            return false; // Ignore clicks on heading or status
        }
    });

    // Function to update an call's data on the server
    const update = async (e) => {
        try {
            let call = JSON.parse(sessionStorage.getItem("call")); // Retrieve the call object from sessionStorage
            call.problemId = parseInt($("#ddlProblem").val());
            call.employeeId = parseInt($("#ddlEmployee").val());
            call.techId = parseInt($("#ddlTechnician").val());
            call.notes = $("#TextNotes").val();

            // If the checkbox is checked (employee is being closed)
            if ($("#checkBoxClosed").is(":checked")) {
                call.openStatus = false;
            } 

            // Send the updated employee data to the server using a PUT request
            let response = await fetch("api/call", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(call),
            });

            if (response.ok) {
                let payload = await response.json();
                $("#status").text(payload.msg); // Display a success message
                getAll(payload.msg);
            } else {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        }
        $("#theModal").modal("toggle"); // Close the modal
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
        loadProblemDDL(-1);
        loadEmployeeDDL(-1);
        loadTechnicianDDL(-1);
        //$("#labelDateClosed").text("");
        $("#TextNotes").val("");
        $("#labelDateClosed").show();
        $("#checkBoxClosed").show();
/*        $("#ddlProblem").val("");
        $("#ddlEmployee").val("");
        $("#ddlTechnician").val("");*/
        //$("#labelDateOpen").text("");
        $("#labelDateClosed").text("");
        $("#checkBoxClosed").attr('checked', false);
        $("#ddlEmployee").attr('disabled', false);
        $("#ddlTechnician").attr('disabled', false);
        $("#ddlProblem").attr('disabled', false);
        $("#textBoxNotes").attr('disabled', false);
        $("#checkBoxClosde").prop('checked', false);
        $("#actionbutton").prop("disabled", false);


        sessionStorage.removeItem("call"); // Clear any previously stored call data

        $("#theModal").modal("toggle"); // Toggle modal visibility
    };

    // Configure modal for adding a new call
    const setupForAdd = () => {
        $("#deletebutton").hide();
        $("#actionbutton").val("add");
        $("#actionbutton").show();
        $("#modaltitle").html("<h4>add call</h4>");
        $("#theModal").modal("toggle");
        $("#modalstatus").text("add new call");
        $("#theModalLabel").text("Add");
        $("#labelDateOpen").show();

        // Set the date opened to the current date and time
        $("#labelDateOpen").text(formatDate().replace("T", " ")); // Format the date to match the UI
        sessionStorage.setItem("dateOpen", formatDate()); // Store the date in session storage

        $("#rowClosed").hide();
        $("#rowClosedCall").hide();
        $("#TextNotes").prop("disabled", false);
        $("#ddlProblem").prop("disabled", false);
        $("#ddlEmployee").prop("disabled", false);
        $("#ddlTechnician").prop("disabled", false);

        
        clearModalFields();
    };

    // Configure modal for updating existing call data
    const setupForUpdate = (id, data) => {
        $("#deletebutton").show(); // Show the delete button
        $("#actionbutton").val("update"); // Set action to "update"
        $("#modaltitle").html("<h4>Update Call</h4>");
        clearModalFields();
        $("#rowClosed").show(); // Show the "Closed" row (if needed)
        $("#rowClosedCall").show();

        $("#checkBoxClosed").prop("disabled", false);
        $("#TextNotes").prop("disabled", false);
        $("#actionbutton").show();
        $("#labelDateClosed").hide();



        // Loop through the data to find the employee by ID
        data.forEach(call => {
            if (call.id === parseInt(id)) {
                sessionStorage.setItem("call", JSON.stringify(call));
                // Populate modal fields with the employee's data
                loadProblemDDL(call.problemId);
                loadEmployeeDDL(call.employeeId);
                loadTechnicianDDL(call.techId);
                $("#labelDateOpen").text(call.dateOpened); // Show the date opened
                $("#TextNotes").val(call.notes);

                if (!call.openStatus) {
                    $("#ddlProblem").prop("disabled", true);
                    $("#ddlEmployee").prop("disabled", true);
                    $("#ddlTechnician").prop("disabled", true);
                    $("#TextNotes").prop("disabled", true);
                    $("#checkBoxClosed").prop("checked", true);
                    $("#checkBoxClosed").prop("disabled", true);
                    $("#labelDateClosed").text(call.dateClosed);
                    $("#labelDateClosed").show();
                    $("#actionbutton").hide();
                }
                else {
                    $("#ddlProblem").prop("disabled", false);
                    $("#ddlEmployee").prop("disabled", false);
                    $("#ddlTechnician").prop("disabled", false);
                    $("#TextNotes").prop("disabled", false);
                    $("#checkBoxClosed").prop("checked", false);
                    $("#checkBoxClosed").prop("disabled", false);
                    $("#labelDateClosed").hide();
                    $("#actionbutton").show();
                }
                // Store the employee data in sessionStorage for later reference
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle"); // Open the modal
                $("#theModalLabel").text("Update Employee");

                $("#actionbutton").on("click", (e) => {
                    buildCallList(data, id);
                });
            }
        });
    };

    // Add a new call to the server
    const add = async () => {
        try {
            let emp = {}; // Create a new call object
            // Populate call properties from the input fields
            emp.ProblemId = parseInt($("#ddlProblem").val());
            emp.EmployeeId = parseInt($("#ddlEmployee").val());
            emp.TechId = parseInt($("#ddlTechnician").val());
            emp.notes = $("#TextNotes").val();
            emp.id = -1;
            emp.timer = null;
            //emp.picture64 = null;

            // Send the call data to the server using a POST request
            let response = await fetch("api/call", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp),
            });
            if (response.ok) {
                let data = await response.json();
                getAll(data.msg); // Refresh the list with the new call
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

    // Delete an call from the server
    const _delete = async () => {
        let call = JSON.parse(sessionStorage.getItem("call"));
        try {
            let response = await fetch(`api/call/${call.id}`, {
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

    $("#checkBoxClosed").on("click", () => {
        if ($("#checkBoxClosed").is(":checked")) {
            $("#labelDateClosed").show();
            $("#labelDateClosed").text(formatDate());
        } else {
            $("#labelDateClosed").text("-");
            //sessionStorage.setItem("dateClosed", "");
        }
        
    }); // checkBoxClose


    // Populate problem dropdown list with data from session storage
    const loadProblemDDL = (empdiv) => {
        let html = '';
        $('#ddlProblem').empty();
        let allproblem = JSON.parse(sessionStorage.getItem('allproblem'));
        allproblem.forEach((div) => { html += `<option value="${div.id}">${div.description}</option>` });
        $('#ddlProblem').append(html);
        $('#ddlProblem').val(empdiv); // Set dropdown to the call's problem if applicable
    };

    const loadEmployeeDDL = (empId) => {
        let html = '';
        $('#ddlEmployee').empty();
        let allemployee = JSON.parse(sessionStorage.getItem('allemployee'));
        allemployee.forEach((emp) => { html += `<option value="${emp.id}">${emp.firstname + " " + emp.lastname}</option>` });
        $('#ddlEmployee').append(html);
        $('#ddlEmployee').val(empId); // Set dropdown to the call's problem if applicable
    };

    const loadTechnicianDDL = (empId) => {
        let html = '';
        $('#ddlTechnician').empty();
        let allemployee = JSON.parse(sessionStorage.getItem('allemployee'));
        allemployee.forEach((emp) => {
            if (emp.isTech) {
                html += `<option value="${emp.id}">${emp.firstname + " " + emp.lastname}</option>`
            }
        });
        $('#ddlTechnician').append(html);
        $('#ddlTechnician').val(empId); // Set dropdown to the call's problem if applicable
    };

    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allcalls"));
        let filtereddata = alldata.filter((call) => call.employeeName.match(new RegExp($("#srch").val(), 'i')));
        buildCallList(filtereddata, false);
    }); // srch keyup

    const formatDate = (date) => {
        let d;
        (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
        let _day = d.getDate();
        if (_day < 10) { _day = "0" + _day; }
        let _month = d.getMonth() + 1;
        if (_month < 10) { _month = "0" + _month; }
        let _year = d.getFullYear();
        let _hour = d.getHours();
        if (_hour < 10) { _hour = "0" + _hour; }
        let _min = d.getMinutes();
        if (_min < 10) { _min = "0" + _min; }
        return _year + "-" + _month + "-" + _day + "T" + _hour + ":" + _min;
    } // formatDate

        document.addEventListener("keyup", e => {
        $("#modalstatus").removeClass(); //remove any existing css on div
        if ($("#EmployeeModalForm").valid()) {
            $("#modalstatus").attr("class", "badge bg-success"); //green
            $("#modalstatus").text("data entered is valid");
            $("#actionbutton").prop("disabled", false);
        }
        else {
            $("#modalstatus").attr("class", "badge bg-danger"); //red
            $("#modalstatus").text("fix errors");
            $("#actionbutton").prop("disabled", true);
        }
    });
    $("#EmployeeModalForm").validate({
        rules: {
            ddlProblem: { required: true },
            ddlEmployee: {required: true },
            ddlTechnician: {required: true },
            TextNotes: { maxlength: 250, required: true},
 
        },
        errorElement: "div",
        messages: {
            ddlProblem: {
                required: "Select Problem"},
            ddlEmployee: {
                required: "Select Employee"},
            ddlTechnician: {
                required: "Select Technician"},
            TextNotes: {
                required: "required 1-250 chars.", maxlength: "required 1-250 chars."
            },
        }
    }); //EmployeeModalForm.validate

}); // jQuery ready method
