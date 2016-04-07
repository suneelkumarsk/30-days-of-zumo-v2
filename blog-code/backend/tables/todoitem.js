var table = require('azure-mobile-apps').table();

table.access = 'authenticated';
table.dynamicSchema = false;
table.columns = {
    userId: 'string',
    text: 'string',
    complete: 'boolean'
};

table.read(function (context) {
    context.query.where({ userId: context.user.emailaddress });
    return context.execute();
});

table.insert(function (context) {
    context.item.userId = context.user.emailaddress;
    return context.execute();
});

table.update(function (context) {
    context.query.where({ userId: context.user.emailaddress });
    context.item.userId = context.user.emailaddress;
    return context.execute();
});

table.delete(function (context) {
    context.query.where({ userId: context.user.emailaddress });
    return context.execute();
});

module.exports = table;
