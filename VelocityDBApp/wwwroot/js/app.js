async function initializeDatabase() {
    try {
        const response = await fetch('/api/data/initialize', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        const result = await response.text();
        document.getElementById('output').innerHTML = `<p>Initialize DB: ${result}</p>`;

        if (!response.ok) {
            console.error('Database initialization failed:', result);
        }
    } catch (error) {
        console.error('Error initializing database:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}

async function getWorkspaceData() {
    try {
        const response = await fetch('/api/data/workspace');

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            document.getElementById('output').innerHTML = `<p>Error: ${errorText}</p>`;
            return;
        }

        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching workspace data:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}

async function getUsers() {
    try {
        const response = await fetch('/api/data/users');

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            document.getElementById('output').innerHTML = `<p>Error: ${errorText}</p>`;
            return;
        }

        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching users:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}

// Schema functions with error handling
async function getDatabaseSchema() {
    try {
        const response = await fetch('/api/schema/database');

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            document.getElementById('output').innerHTML = `<p>Error: ${errorText}</p>`;
            return;
        }

        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching database schema:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}

async function getDataStructure() {
    try {
        const response = await fetch('/api/schema/structure');

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            document.getElementById('output').innerHTML = `<p>Error: ${errorText}</p>`;
            return;
        }

        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching data structure:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}

async function getTypeSchema(typeName) {
    try {
        const response = await fetch(`/api/schema/type/${typeName}`);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            document.getElementById('output').innerHTML = `<p>Error: ${errorText}</p>`;
            return;
        }

        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching type schema:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}

async function getAvailableTypes() {
    try {
        const response = await fetch('/api/schema/types');

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            document.getElementById('output').innerHTML = `<p>Error: ${errorText}</p>`;
            return;
        }

        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching available types:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}

async function checkDatabaseStatus() {
    try {
        const response = await fetch('/api/data/status');

        if (!response.ok) {
            const errorText = await response.text();
            document.getElementById('output').innerHTML = `<p>Error: ${errorText}</p>`;
            return;
        }

        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error checking database status:', error);
        document.getElementById('output').innerHTML = `<p>Error: ${error.message}</p>`;
    }
}