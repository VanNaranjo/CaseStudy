    $(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding Employee Information...");
            let response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it
                buildEmployeeList(payload);
                msg === "" ? // are we appending to an existing message
                    $("#status").text("Employees Loaded") : $("#status").text(`${msg} - Employees Loaded`);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll
    const buildEmployeeList = (data) => {
        $("#employeeList").empty();
        div = $(`<div class="list-group-item text-black bg-primary row d-flex" id="status">Employee Info</div>
         <div class= "list-group-item row d-flex text-center text-primary" id="heading">
         <div class="col-4 h4">Title</div>
         <div class="col-4 h4">First</div>
         <div class="col-4 h4">Last</div>
         </div>`);
        div.appendTo($("#employeeList"));
        sessionStorage.setItem("allemployees", JSON.stringify(data));
        data.forEach(stu => {
            btn = $(`<button class="list-group-item row d-flex text-info" id="${stu.id}">`);
            btn.html(`<div class="col-4" id="employeetitle${stu.id}">${stu.title}</div>
         <div class="col-4" id="employeefname${stu.id}">${stu.firstname}</div>
         <div class="col-4" id="employeeelastnam${stu.id}">${stu.lastname}</div>`
            );
            btn.appendTo($("#employeeList"));
        }); // forEach
    }; // buildEmployeeList
    getAll(""); // first grab the data from the server

    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        } // clicked on row somewhere else
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allemployees"));
            data.forEach(employee => {
                if (employee.id === parseInt(id)) {
                    $("#TextBoxTitle").val(employee.title);
                    $("#TextBoxFirst").val(employee.firstname);
                    $("#TextBoxLast").val(employee.lastname);
                    $("#TextBoxEmail").val(employee.email);
                    $("#TextBoxPhone").val(employee.phoneno);
                    sessionStorage.setItem("employee", JSON.stringify(employee));
                    $("#modalstatus").text("update data");
                    $("#theModal").modal("toggle");
                } // if
            }); // data.map
        } else {
            return false; // ignore if they clicked on heading or status
        }
    }); // employeeListClick

    $("#actionbutton").on('click', async (e) => {
        // action button click event handler
        try {
            // set up a new client side instance of Employee
            let stu = JSON.parse(sessionStorage.getItem("employee"));
            // pouplate the properties
            stu.title = $("#TextBoxTitle").val();
            stu.firstname = $("#TextBoxFirst").val();
            stu.lastname = $("#TextBoxLast").val();
            stu.email = $("#TextBoxEmail").val();
            stu.phoneno = $("#TextBoxPhone").val();
            // send the updated back to the server asynchronously using Http PUT
            let response = await fetch("api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(stu),
            });
            if (response.ok) {
                // or check for response.status
                let payload = await response.json();
                $("#status").text(payload.msg);
            } else if (response.status !== 404) {
                // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        } // try/catch
    }); // action button click

    const errorRtn = (problemJson, status) => {
        if (status > 499) {
            $("#status").text("Problem server side, see debug console");
        } else {
            let keys = Object.keys(problemJson.errors)
            problem = {
                status: status,
                statusText: problemJson.errors[keys[0]][0], // first error
            };
            $("#status").text("Problem client side, see browser console");
            console.log(problem);
        } // else
    }

}); // jQuery ready method

