import { test, expect } from '@playwright/test';

test.describe('MBD Admin Portal Condition Deletion Test', () => {
  const timestamp = Date.now();
  const testConditionName = `TestConditionDel${timestamp}`;

  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    page.on('dialog', async dialog => {
        console.log(`Native Dialog message: ${dialog.message()}`);
        await dialog.accept();
    });
  });

  test('should not open modal after deleting a condition', async ({ page }) => {
    test.setTimeout(60000);

    // Navigate to conditions tab
    await page.getByRole('button', { name: 'conditions', exact: true }).click();

    // Create a condition
    await page.getByRole('button', { name: '+ Add New Condition' }).click();
    await page.locator('#name').fill(testConditionName);
    await page.getByRole('button', { name: 'Save Condition' }).click();

    // Handle potential error modal (metadata update 500)
    const errorModal = page.getByText('Error Saving Condition');
    const okButton = page.getByRole('button', { name: 'OK' });
    try {
        await expect(errorModal).toBeVisible({ timeout: 5000 });
        await okButton.click();
        await expect(errorModal).not.toBeVisible();
        const closeEditModal = page.getByRole('button', { name: 'Cancel' });
        if (await closeEditModal.isVisible()) {
            await closeEditModal.click();
        }
    } catch (e) {
        // No error modal, good
    }

    // Verify creation
    await page.getByRole('button', { name: 'Refresh Conditions' }).click();
    await page.getByPlaceholder('Search by Name, Physical Connections, or Tags...').fill(testConditionName);
    const conditionRow = page.locator('tr').filter({ hasText: testConditionName }).first();
    await expect(conditionRow).toBeVisible({ timeout: 10000 });

    // Delete condition
    console.log(`Deleting condition: ${testConditionName}`);
    await conditionRow.getByRole('button', { name: 'Delete' }).click();

    // Verify row is gone
    await expect(conditionRow).not.toBeVisible({ timeout: 10000 });

    // CRITICAL CHECK: Verify modal is NOT visible
    // The modal title is "Add New Condition" because addMbdCondition() resets the state.
    // Or if it was an edit modal, it would have the name.
    // My fix prevented addMbdCondition() from being called, so the modal state shouldn't change to "show".
    await expect(page.getByText('Add New Condition')).not.toBeVisible();
    await expect(page.getByText('Edit: ' + testConditionName)).not.toBeVisible();
  });
});
