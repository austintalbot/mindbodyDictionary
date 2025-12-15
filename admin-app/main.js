const { app, BrowserWindow } = require('electron');
const path = require('path');

function createWindow () {
  const mainWindow = new BrowserWindow({
    width: 1200,
    height: 900,
    webPreferences: {
      preload: path.join(__dirname, 'electron/preload.js'),
      // WARNING: Consider security implications.
      // `nodeIntegration: true` allows renderer process to use Node.js APIs.
      // `contextIsolation: false` means Node.js APIs are available directly in the renderer's global scope.
      // For production apps, it's generally recommended to use `contextIsolation: true`
      // and expose necessary Node.js APIs via the `preload.js` script.
      nodeIntegration: true, 
      contextIsolation: false,
    }
  });

  // In development, load the React app from the Vite dev server.
  // In production, load the built React app.
  const startUrl = process.env.NODE_ENV === 'development'
    ? 'http://localhost:5173' // Default Vite dev server port
    : `file://${path.join(__dirname, 'dist', 'index.html')}`;

  mainWindow.loadURL(startUrl);

  // Open the DevTools.
  // mainWindow.webContents.openDevTools();
}

app.whenReady().then(() => {
  createWindow();

  app.on('activate', function () {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', function () {
  if (process.platform !== 'darwin') app.quit();
});
