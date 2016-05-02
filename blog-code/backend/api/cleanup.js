var allowedUsers = [
    'photoadrian@outlook.com'
];

var api = {
    /**
     * POST /api/cleanup - deletes records during cleanup
     */
    post: function (req, res, next) {

        // Authorization - you must be in the allowed set of users
        if (allowedUsers.indexof(req.user.emailaddress) == -1) {
            res.status(401).send({ error: 'Unauthorized' });
            return;
        }

        // Now execute the SQL Query
        var query = {
            sql: 'DELETE FROM TodoItem WHERE deleted = 1 AND updatedAt < DATEADD(d, -7, GETDATE())',
            parameters: []
        };

        req.azureMobile.data.execute(query).then(function (results) {
            response.json(results);
        });
    },
};

api.post.access = 'authenticated';

module.exports = api;
