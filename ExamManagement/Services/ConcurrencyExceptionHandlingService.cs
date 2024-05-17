using ExamManagement.Data;
using ExamManagement.Models;
using ExamManagement.Models.Domains;
using ExamManagement.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace ExamManagement.Services // Adjust namespace as needed
{
    public class ConcurrencyExceptionHandlingService
    {
        private readonly SgsDbContext _context;

        public ConcurrencyExceptionHandlingService(SgsDbContext context)
        {
            _context = context;
        }


        /// Summary: Handles concurrency exceptions (DbUpdateConcurrencyException) that can occur when multiple users try to update the same entity.
        /// <typeparam name="T">The type of the entity being updated.</typeparam>
        /// <param name="entity">The entity that was attempted to be updated.</param>
        /// <param name="retrySaveAction">A function that performs the update logic and should be retried if a concurrency exception occurs.</param>
        /// <param name="modelState">The ModelStateDictionary to add concurrency errors to.</param>
        /// <returns>An IActionResult indicating the result of the operation (success or conflict).</returns>

        // Method to handle concurrency exceptions
        public async Task<IActionResult> HandleConcurrencyExceptionAsync<T>(T entity, Func<Task> retrySaveAction, ModelStateDictionary modelState) where T : class
        {
            try
            {
                // Attempt to save the changes.
                await retrySaveAction();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Concurrency conflict detected
                var entry = _context.Entry(entity);

                // Check if the entity was deleted by another user
                if (entry.State == EntityState.Detached)
                {
                    return new NotFoundObjectResult("Entity not found."); // 404 Not Found
                }

                // Get the current database values for comparison
                var databaseValues = await entry.GetDatabaseValuesAsync();
                var concurrencyErrors = new List<ConcurrencyError>(); // To store concurrency errors

                // Compare original and current values
                foreach (var property in databaseValues.Properties)
                {
                    var propertyName = property.Name;

                    var originalValue = entry.OriginalValues[propertyName]?.ToString();
                    var databaseValue = databaseValues[propertyName]?.ToString();

                    if (databaseValue != originalValue)
                    {
                        // Create a ConcurrencyError object
                        var error = new ConcurrencyError
                        {
                            PropertyName = propertyName,
                            ErrorMessage = $"The value of '{propertyName}' was modified since you loaded the page. The current value is '{databaseValue}'."
                        };
                        concurrencyErrors.Add(error);
                    }

                    // Update original values to current values
                    entry.OriginalValues[propertyName] = databaseValues[propertyName];
                }

                // If any conflicts were found, return a BadRequest with error details
                if (concurrencyErrors.Count > 0)
                {
                    return new BadRequestObjectResult(concurrencyErrors); // 400 Bad Request
                }

                // Retry saving changes
                await retrySaveAction();
            }

            // If no concurrency exceptions, return a success response
            return new NoContentResult(); // 204 No Content
        }
    }

    /* Scenario: 
        1. Initial State: A `Question` with `Id = 1` exists in the database with `TotalPoints = 10`.
        2. User 1 Action: User 1 retrieves the question and intends to change `TotalPoints` to `20`.
        3. User 2 Action: User 2, unaware of User 1's action, also fetches the same question and modifies `TotalPoints` to `15`.
        4. Update Race: User 2 submits their changes first, successfully updating `TotalPoints` to `15` in the database.
        5. Conflict: When User 1 tries to save their changes, they encounter a concurrency conflict because the data they originally fetched is no longer in sync with the database.
    */

    /*  Ooga Booga explanation
        Note that: The fetched data don't get Update if there a Update. It just store the data at the fetched point
        Database
                | TotalPoints: 10 |  <--- User 1 loads Question (original value = 10)
                | TotalPoints: 15 |  <--- User 2 updates and saves (new value = 15) but wait, User1 fetched data haven't been Updated
        User 1 tries to save (value = 20) -> CONFLICT!

        ConcurrencyExceptionHandlingService:
        * Detects conflict
        * Creates ConcurrencyError object

        Controller:
        * Returns BadRequest with ConcurrencyError details
     */

}