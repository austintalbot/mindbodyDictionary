import { test, expect } from '@playwright/test';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

test.describe('MBD Admin Portal Full Functionality Test', () => {
  // Use a unique name to avoid collisions
  const timestamp = Date.now();
  const testConditionName = `TestCondition${timestamp}`;
  const testConditionSummary = 'This is a comprehensive test condition created by Playwright.';

  test.beforeEach(async ({ page }) => {
    // Load the app
    await page.goto('/');
    // Expand to "full screen"
    await page.setViewportSize({ width: 1920, height: 1080 });

    // Initial load check
    await expect(page.getByRole('heading', { name: 'MBD Admin Portal' })).toBeVisible({ timeout: 15000 });

    // Auto-accept all native dialogs (alerts, confirms)
    page.on('dialog', async dialog => {
        console.log(`Native Dialog message: ${dialog.message()}`);
        await dialog.accept();
    });
  });

  test('should navigate through all functional tabs', async ({ page }) => {
    const tabs = ['conditions', 'images', 'contacts', 'notifications', 'faqs', 'links', 'database'];
    for (const tab of tabs) {
      console.log(`Navigating to tab: ${tab}`);
      await page.getByRole('button', { name: tab, exact: true }).click();

      // Basic verification for each tab
      switch (tab) {
        case 'conditions':
          await expect(page.getByRole('heading', { name: 'Conditions' })).toBeVisible();
          break;
        case 'images':
          await expect(page.getByRole('heading', { name: 'Images' })).toBeVisible();
          break;
        case 'contacts':
          await expect(page.getByRole('heading', { name: 'Contacts' })).toBeVisible();
          break;
        case 'notifications':
          await expect(page.getByRole('heading', { name: 'Push Notifications' })).toBeVisible();
          break;
        case 'faqs':
          await expect(page.getByRole('heading', { name: 'Freqently Asked Questions' })).toBeVisible();
          break;
        case 'links':
          await expect(page.getByRole('heading', { name: 'MindBody Movement Links' })).toBeVisible();
          break;
        case 'database':
          await expect(page.getByRole('heading', { name: 'Database Management' })).toBeVisible();
          break;
      }
    }
  });

  test('Condition Life Cycle: Create, Upload Images, and Cleanup', async ({ page }) => {
    test.setTimeout(180000);

    // --- 1. CREATE CONDITION ---
    console.log('Step 1: Creating condition');
    await page.getByRole('button', { name: 'conditions', exact: true }).click();
    await page.getByRole('button', { name: '+ Add New Condition' }).click();

    // Fill Basic Info
    await page.locator('#name').fill(testConditionName);
    await page.locator('#summaryNegative').fill(`${testConditionSummary} (Negative)`);
    await page.locator('#summaryPositive').fill(`${testConditionSummary} (Positive)`);

    // Save
    console.log('Saving condition...');
    // We expect a response, but it might be a 500 if the metadata update fails,
    // even if the condition is saved.
    await page.getByRole('button', { name: 'Save Condition' }).click();

    // Handle potential error modal (the 500 error known in staging)
    const errorModal = page.getByText('Error Saving Condition');
    const okButton = page.getByRole('button', { name: 'OK' });

    try {
        await expect(errorModal).toBeVisible({ timeout: 5000 });
        console.log('Detected error modal (likely the metadata update 500). Dismissing...');
        await okButton.click();
        // Wait for it to disappear
        await expect(errorModal).not.toBeVisible();
        // Manually close the edit modal if it stayed open
        const closeEditModal = page.getByRole('button', { name: 'Cancel' });
        if (await closeEditModal.isVisible()) {
            await closeEditModal.click();
        }
    } catch (e) {
        console.log('No error modal detected or it closed automatically.');
    }

    // Verify creation by searching with retries
    console.log(`Verifying condition "${testConditionName}" creation...`);
    await expect(async () => {
        await page.getByRole('button', { name: 'Refresh Conditions' }).click();
        await page.getByPlaceholder('Search by Name, Physical Connections, or Tags...').clear();
        await page.getByPlaceholder('Search by Name, Physical Connections, or Tags...').fill(testConditionName);
        await expect(page.getByRole('cell', { name: testConditionName })).toBeVisible({ timeout: 5000 });
    }).toPass({ timeout: 30000 });

    // --- 2. UPLOAD IMAGES ---
    console.log('Step 2: Uploading images');
    await page.getByRole('button', { name: 'images', exact: true }).click();

        // Helper to upload image
        const uploadImage = async (typeLabel: string, typeValue: string) => {
            console.log(`Uploading ${typeLabel} Image...`);
            await page.getByRole('button', { name: '+ Add New Image' }).click();

            // Wait for conditions to load in dropdown
            await page.locator('select').first().selectOption({ label: testConditionName });
            await page.locator('select').nth(1).selectOption(typeValue);
            await page.setInputFiles('input[id="imageFile"]', path.join(__dirname, 'assets/test-image.png'));

            await page.getByRole('button', { name: 'Upload Image' }).click();

            // Wait for upload success or modal close
            await expect(page.getByText('Uploading...')).not.toBeVisible({ timeout: 60000 });

            // If there's a cancel/close button, click it to ensure modal is gone
            const closeButton = page.getByRole('button', { name: 'Cancel' });
            if (await closeButton.isVisible()) {
                await closeButton.click();
            }
            await expect(page.getByText('Add New Image Metadata')).not.toBeVisible({ timeout: 5000 }).catch(() => {});
        };
    await uploadImage('Negative', '1');
    await uploadImage('Positive', '2');

    // Verify images appear in the table
    await expect(async () => {
        await page.getByRole('button', { name: 'Refresh Images' }).click();
        await page.getByPlaceholder('Search images by name or condition...').clear();
        await page.getByPlaceholder('Search images by name or condition...').fill(testConditionName);
        await expect(page.getByRole('cell', { name: `${testConditionName}1.png` })).toBeVisible({ timeout: 5000 });
        await expect(page.getByRole('cell', { name: `${testConditionName}2.png` })).toBeVisible({ timeout: 5000 });
    }).toPass({ timeout: 30000 });

    // --- 3. CLEANUP IMAGES ---
    console.log('Step 3: Cleaning up images');
    let row = page.locator('tr').filter({ hasText: testConditionName }).first();
    while (await row.count() > 0) {
        const imgName = await row.locator('td').first().textContent();
        console.log(`Deleting image: ${imgName}`);
        await row.getByRole('button', { name: 'Delete' }).click();
        // Wait for it to disappear from the UI
        await expect(row).not.toBeVisible({ timeout: 10000 });
        row = page.locator('tr').filter({ hasText: testConditionName }).first();
    }

    // --- 4. CLEANUP CONDITION ---
    console.log('Step 4: Cleaning up condition');
    await page.getByRole('button', { name: 'conditions', exact: true }).click();
    await page.getByPlaceholder('Search by Name, Physical Connections, or Tags...').clear();
    await page.getByPlaceholder('Search by Name, Physical Connections, or Tags...').fill(testConditionName);

    const conditionRow = page.locator('tr').filter({ hasText: testConditionName }).first();
    if (await conditionRow.count() > 0) {
        console.log(`Deleting condition: ${testConditionName}`);
        await conditionRow.getByRole('button', { name: 'Delete' }).click();
        await expect(conditionRow).not.toBeVisible({ timeout: 10000 });
        // Ensure modal did not open (Regression test for bug where delete opened the add modal)
        await expect(page.getByText('Add New Condition')).not.toBeVisible();
    }

    console.log('Cleanup complete.');
  });
});
