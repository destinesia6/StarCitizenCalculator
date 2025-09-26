namespace StarCitizenCalculator;

using System;
using System.Collections.Generic;
using System.Linq;

// --- Data Structures (Unchanged, for clarity) ---

/// <summary>
/// Represents a single delivery job for a resource.
/// </summary>
public class DeliveryJob
{
    public required string ResourceName { get; init; }

    public Dictionary<string, int> Drops { get; set; } = new();

    /// <summary>
    /// Calculates the total amount of the resource needed for this job.
    /// </summary>
    public int TotalNeeded => Drops.Values.Sum();
}

/// <summary>
/// Represents the box sizes for a given amount.
/// </summary>
public class BoxCount
{
    public int Box4 { get; set; }
    public int Box2 { get; set; }
    public int Box1 { get; set; }

    public override string ToString() => $"4u: {Box4}, 2u: {Box2}, 1u: {Box1}";
}

public static class Calculator
{
	  private static BoxCount CalculateBoxes(int amount)
    {
        BoxCount boxes = new();
        if (amount <= 0) return boxes;

        boxes.Box4 = amount / 4;
        amount %= 4;

        boxes.Box2 = amount / 2;
        amount %= 2;

        boxes.Box1 = amount;

        return boxes;
    }

    public static void ProcessJobs(List<DeliveryJob> jobs, List<string> locationNames)
    {
        // Data structure for the total drops at each location
        Dictionary<string, List<(string Resource, BoxCount Boxes)>> locationDrops = locationNames.ToDictionary(name => name, _ => new List<(string, BoxCount)>());

        // Data structure for the total pickups needed
        List<(string Resource, BoxCount Boxes)> pickupSummary = [];

        int jobCounter = 1;
        foreach (DeliveryJob job in jobs)
        {
            // Give a unique name (e.g., Carbon 1, Iron 2)
            string uniqueResourceName = $"{job.ResourceName} {jobCounter++}";

            // 1. Calculate Pickups (based on TotalNeeded)
            BoxCount pickupBoxes = CalculateBoxes(job.TotalNeeded);
            pickupSummary.Add((uniqueResourceName, pickupBoxes));

            // 2. Calculate Drops
            foreach ((string location, int dropAmount) in job.Drops) if (dropAmount > 0) locationDrops[location].Add((uniqueResourceName, CalculateBoxes(dropAmount)));
        }

        // --- Output ---
        PrintPickupSummary(pickupSummary);
        PrintLocationDrops(locationDrops);
    }

    // --- Output Functions (Unchanged) ---

    private static void PrintPickupSummary(List<(string Resource, BoxCount Boxes)> summary)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("                RESOURCE PICKUP SUMMARY                ");
        Console.WriteLine("=======================================================");
        Console.ResetColor();
        Console.WriteLine("This is the breakdown of what boxes to collect for each job:");

        foreach ((string resource, BoxCount boxes) in summary.OrderBy(s => s.Resource))
        {
            Console.WriteLine($"  - {resource,-15}: Total Units: {boxes.Box4 * 4 + boxes.Box2 * 2 + boxes.Box1} | {boxes}");
        }
        Console.WriteLine("=======================================================\n");
    }

    private static void PrintLocationDrops(Dictionary<string, List<(string Resource, BoxCount Boxes)>> drops)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=======================================================");
        Console.WriteLine("                LOCATION DROPOFF LIST                  ");
        Console.WriteLine("=======================================================");
        Console.ResetColor();
        Console.WriteLine("This is the breakdown of what boxes to drop at each location:");

        foreach ((string location, List<(string Resource, BoxCount Boxes)> dropList) in drops.OrderBy(d => d.Key))
        {
	        if (dropList.Count == 0) continue;
	        Console.ForegroundColor = ConsoleColor.Green;
	        Console.WriteLine($"\n--- Location: {location} ---");
	        Console.ResetColor();

	        foreach ((string resource, BoxCount boxes) in dropList.OrderBy(d => d.Resource))
	        {
		        Console.WriteLine($"  - {resource,-15}: Drop Units: {boxes.Box4 * 4 + boxes.Box2 * 2 + boxes.Box1} | {boxes}");
	        }
        }
        Console.WriteLine("\n=======================================================");
    }
}

internal static class Program
{
	private static void Main()
    {
        Console.Title = "Star Citizen Delivery Calculator";
        List<string> locationNames = [];
        List<DeliveryJob> jobs = [];

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("========== Star Citizen Delivery Calculator ==========");
        Console.ResetColor();

        // --- Input 1: Location Names ---
        Console.WriteLine("\nFirst, enter the names of your 4 delivery locations.");
        for (int i = 0; i < 4; i++)
        {
            Console.Write($"Enter name for Location {i + 1}: ");
            string? name = Console.ReadLine()?.Trim();
            if (name is null) throw new NullReferenceException("A value for name was not entered");
            // Ensure location names are not empty
            locationNames.Add(String.IsNullOrWhiteSpace(name) ? $"Location {i + 1}" : name);
        }

        Console.WriteLine($"\nLocations set: {String.Join(", ", locationNames)}");
        Console.WriteLine("---------------------------------------------");
        
        // --- Input 2: Job Data Loop ---
        Console.WriteLine("\nNow, enter the job details. Type 'done' for the resource name when finished.");
        int jobCount = 1;
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n--- Job {jobCount} ---");
            Console.ResetColor();

            Console.Write("Enter Resource Name (or type 'done' to finish): ");
            string? resourceName = Console.ReadLine()?.Trim();
            if (resourceName is null) throw new NullReferenceException("A value for name was not entered");

            if (resourceName.Equals("done", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            if (String.IsNullOrWhiteSpace(resourceName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Resource name cannot be empty. Please try again.");
                Console.ResetColor();
                continue;
            }

            DeliveryJob newJob = new() { ResourceName = resourceName };
            int totalUnits = 0;

            foreach (string location in locationNames)
            {
                int dropAmount = 0;
                bool validInput = false;

                while (!validInput)
                {
                    Console.Write($"  Amount to drop at {location} (units, default 0): ");
                    string? input = Console.ReadLine()?.Trim();
                    if (input is null) throw new NullReferenceException("A value for name was not entered");
                    
                    if (String.IsNullOrWhiteSpace(input)) input = "0";

                    if (Int32.TryParse(input, out dropAmount) && dropAmount >= 0)
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input. Please enter a non-negative whole number.");
                        Console.ResetColor();
                    }
                }
                newJob.Drops.Add(location, dropAmount);
                totalUnits += dropAmount;
            }

            if (totalUnits > 0)
            {
                jobs.Add(newJob);
                jobCount++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Job for {resourceName} skipped: Total units to deliver was 0.");
                Console.ResetColor();
            }
        }

        if (jobs.Count != 0)
        {
            // Run the main processing logic
            Calculator.ProcessJobs(jobs, locationNames);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNo jobs were entered. Exiting application.");
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}