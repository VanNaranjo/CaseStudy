/*
Van Naranjo
December 9, 2024
This is the js file for employeepdf.html that will print out reports
*/

$(() => {
    // Event listener for when the employee report button is clicked
    $("#employeebutton").on("click", async (e) => {
        try {
            // Set the status label to inform the user that the report is being generated
            $("#employeelblstatus").text("generating report on server - please wait...");

            // Fetch the employee report from the server
            let response = await fetch(`api/employeereport`);

            // If the response is not OK (status code other than 200), throw an error
            if (!response.ok)
                throw new Error(
                    `Status - ${response.status}, Text - ${response.statusText}`
                );

            // Parse the response data as JSON
            let data = await response.json(); // this returns a promise, so we await it

            // Update the status label to indicate that the report has been generated
            $("#employeelblstatus").text("report generated");

            // If the message in the response data indicates the report was generated, open the PDF
            data.msg === "Report Generated"
                ? window.open("/pdfs/employeereport.pdf")
                : $("#employeelblstatus").text("problem generating report"); // Otherwise, display an error message
        } catch (error) {
            // If an error occurs, display the error message in the status label
            $("#employeelblstatus").text(error.message);
        } // end try/catch block
    }); // end employee button click handler

    // Event listener for when the call report button is clicked
    $("#callbutton").on("click", async (e) => {
        try {
            // Set the status label to inform the user that the report is being generated
            $("#calllblstatus").text("generating report on server - please wait...");

            // Fetch the call report from the server
            let response = await fetch(`api/callreport`);

            // If the response is not OK (status code other than 200), throw an error
            if (!response.ok)
                throw new Error(
                    `Status - ${response.status}, Text - ${response.statusText}`
                );

            // Parse the response data as JSON
            let data = await response.json(); // this returns a promise, so we await it

            // Update the status label to indicate that the report has been generated
            $("#calllblstatus").text("report generated");

            // If the message in the response data indicates the report was generated, open the PDF
            data.msg === "Report Generated"
                ? window.open("/pdfs/callreport.pdf")
                : $("#calllblstatus").text("problem generating report"); // Otherwise, display an error message
        } catch (error) {
            // If an error occurs, display the error message in the status label
            $("#calllblstatus").text(error.message);
        } // end try/catch block
    }); // end call button click handler
}); // end document ready function