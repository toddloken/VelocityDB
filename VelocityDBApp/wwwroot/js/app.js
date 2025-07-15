async function initializeDatabase() {
    try {
        const response = await fetch('/api/data/initialize', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        const result = await response.text();
        document.getElementById('output').innerHTML = `<p>${result}</p>`;
    } catch (error) {
        console.error('Error initializing database:', error);
    }
}

async function getWorkspaceData() {
    try {
        const response = await fetch('/api/data/workspace');
        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching workspace data:', error);
    }
}

async function getUsers() {
    try {
        const response = await fetch('/api/data/users');
        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching users:', error);
    }
}

async function getDatabaseSchema() {
    try {
        const response = await fetch('/api/schema/database');
        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching database schema:', error);
    }
}

async function getDataStructure() {
    try {
        const response = await fetch('/api/schema/structure');
        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching data structure:', error);
    }
}

async function getTypeSchema(typeName) {
    try {
        const response = await fetch(`/api/schema/type/${typeName}`);
        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching type schema:', error);
    }
}

async function getAvailableTypes() {
    try {
        const response = await fetch('/api/schema/types');
        const data = await response.json();
        document.getElementById('output').innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
    } catch (error) {
        console.error('Error fetching available types:', error);
    }
}