(function () {
    "use strict";

    var client,         // Connection to the Azure Mobile app backed
        todoItemTable;  // Reference to the table endpoint

    $(document).ready(onBrowserReady);

    /**
     * Event handler, called when the browser has loaded all scripts
     * @event
     */
    function onBrowserReady() {
        // Create a connection reference to our Azure Mobile Apps backend
        client = new WindowsAzure.MobileServiceClient('https://30-days-of-zumo-v2.azurewebsites.net');

        // Create a table reference
        todoItemTable = client.getTable('todoitem');

        // Success and failure
        function authSuccess(data) {
            console.info('Authentication succeeded: data = ', data);
            initializeApp();
        }

        function authFailed(error) {
            console.error('Authentication failed: error = ', error);
            alert('AUTH FAILED!');
        }

        // Wire up the button to initialize the application
        $('#loginButton').on('click', function (event) {
            event.preventDefault();
            client.login('aad').then(authSuccess, authFailed);
        });
    }

    /**
     * Called after the entry button is clicked to clean up the old HTML and add our HTML
     */
    function initializeApp() {
        $('#wrapper').empty();

        // Replace the wrapper with the main content from the original Todo App
        var content =
              '<div id="wrapper">'
            + '<article>'
            + '  <header>'
            + '    <h2>Azure</h2><h1>Mobile Apps</h1>'
            + '    <form id="add-item">'
            + '      <button id="refresh">Refresh</button>'
            + '      <button id="add">Add</button>'
            + '      <div><input type="text" id="new-item-text" placeholder="Enter new task" /></div>'
            + '    </form>'
            + '  </header>'
            + '  <ul id="todo-items"></ul>'
            + '  <p id="summary">Initializing...</p>'
            + '</article>'
            + '<footer><ul id="errorlog"></ul></footer>'
            + '</div>';
        $('#page').html(content);

        // Refresh the todoItems
        refreshDisplay();

        // Wire up the UI Event Handler for the Add Item
        $('#add').on('click', addItemHandler);
        $('#refresh').on('click', handleRefresh);
    }

    /**
     * Refresh the items within a page
     */
    function refreshDisplay() {
        updateSummaryMessage('Loading data from Azure');

        // Execute a query for uncompleted items and process
        todoItemTable
            .where({ complete: false })     // Set up the query
            .read()                         // Send query and read results
            .then(createTodoItemList, handleError);
    }

    /**
     * Updates the Summary Message
     * @param {string} msg the message to use
     * @returns {void}
     */
    function updateSummaryMessage(msg) {
        $('#summary').html(msg);
    }

    /**
     * Create the DOM for a single todo item
     * @param {Object} item the Todo item
     * @param {string} item.id the ID of the item
     * @param {bool} item.complete true if the item is complete
     * @param {string} item.text the text value
     * @returns {jQuery} jQuery DOM object
     */
    function createTodoItem(item) {
        return $('<li>')
            .attr('data-todoitem-id', item.id)
            .append($('<button class="item-delete">Delete</button>'))
            .append($('<input type="checkbox" class="item-complete">')
                .prop('checked', item.complete))
            .append($('<div>')
                .append($('<input class="item-text">').val(item.text)));
    }

    /**
     * Create a list of Todo Items
     * @param {TodoItem[]} items an array of items
     * @returns {void}
     */
    function createTodoItemList(items) {
        // Cycle through each item received from Azure and add items to the item list
        var listItems = $.map(items, createTodoItem);
        $('#todo-items').empty().append(listItems).toggle(listItems.length > 0);
        updateSummaryMessage('<strong>' + items.length + '</strong> item(s)');

        // Wire up event handlers
        $('.item-delete').on('click', deleteItemHandler);
        $('.item-text').on('change', updateItemTextHandler);
        $('.item-complete').on('change', updateItemCompleteHandler);
    }

    /**
     * Handle error conditions
     * @param {Error} error the error that needs handling
     * @returns {void}
     */
    function handleError(error) {
        console.error('handleError: ', error);
        var text = error + (error.request ? ' - ' + error.request.status : '');
        $('#errorlog').append($('<li>').text(text));
    }

    /**
     * Given a sub-element of an LI, find the TodoItem ID associated with the list member
     *
     * @param {DOMElement} el the form element
     * @returns {string} the ID of the TodoItem
     */
    function getTodoItemId(el) {
        return $(el).closest('li').attr('data-todoitem-id');
    }

    /**
     * Event handler for when the user enters some text and clicks on Add
     * @param {Event} event the event that caused the request
     * @returns {void}
     */
    function addItemHandler(event) {
        var textbox = $('#new-item-text'),
            itemText = textbox.val();

        updateSummaryMessage('Adding New Item');
        if (itemText !== '') {
            todoItemTable.insert({
                text: itemText,
                complete: false
            }).then(refreshDisplay, handleError);
        }

        textbox.val('').focus();
        event.preventDefault();
    }

    /**
     * Event handler for when the user clicks on Delete next to a todo item
     * @param {Event} event the event that caused the request
     * @returns {void}
     */
    function deleteItemHandler(event) {
        var itemId = getTodoItemId(event.currentTarget);

        updateSummaryMessage('Deleting Item in Azure');
        console.info('deleteItemHandler: itemId = ', itemId);
        todoItemTable
            .del({ id: itemId })   // Async send the deletion to backend
            .then(refreshDisplay, handleError); // Update the UI
        event.preventDefault();
    }

    /**
     * Event handler for when the user updates the text of a todo item
     * @param {Event} event the event that caused the request
     * @returns {void}
     */
    function updateItemTextHandler(event) {
        var itemId = getTodoItemId(event.currentTarget),
            newText = $(event.currentTarget).val();

        updateSummaryMessage('Updating Item in Azure');
        console.info('updateItemTextHandler: itemId = ', itemId);
        todoItemTable
            .update({ id: itemId, text: newText })  // Async send the update to backend
            .then(refreshDisplay, handleError); // Update the UI
        event.preventDefault();
    }

    /**
     * Event handler for when the user updates the completed checkbox of a todo item
     * @param {Event} event the event that caused the request
     * @returns {void}
     */
    function updateItemCompleteHandler(event) {
        var itemId = getTodoItemId(event.currentTarget),
            isComplete = $(event.currentTarget).prop('checked');

        updateSummaryMessage('Updating Item in Azure');
        console.info('updateItemCompleteHandler: itemId = ', itemId);
        todoItemTable
            .update({ id: itemId, complete: isComplete })  // Async send the update to backend
            .then(refreshDisplay, handleError);        // Update the UI
    }
})();
