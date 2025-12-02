namespace StarCitizenCalculator;

using System.Collections.Generic;
using System.Linq;

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

	// Structured Data for Pickups
	public record PickupSummaryItem(string Resource, BoxCount Boxes, int TotalUnits);

	// Structured Data for Drops
	public record DropSummaryItem(string Resource, BoxCount Boxes, int DropUnits);

	// Main processing method that returns structured data for the manifest
	public static (List<PickupSummaryItem> PickupSummary, Dictionary<string, List<DropSummaryItem>> LocationDrops) ProcessJobs(List<DeliveryJob> jobs, List<string> locationNames)
	{
		Dictionary<string, List<DropSummaryItem>> locationDrops = locationNames.ToDictionary(name => name, _ => new List<DropSummaryItem>());
		List<PickupSummaryItem> pickupSummary = [];
		int jobCounter = 1;

		foreach (DeliveryJob job in jobs)
		{
			string uniqueResourceName = $"{job.ResourceName} {jobCounter++}";

			// 1. Calculate Pickups
			BoxCount pickupBoxes = new();

			// 2. Calculate Drops
			foreach ((string location, int dropAmount) in job.Drops)
			{
				if (dropAmount > 0)
				{
					BoxCount dropoffBoxes = CalculateBoxes(dropAmount);
					locationDrops[location].Add(new DropSummaryItem(uniqueResourceName, dropoffBoxes, dropAmount));
					pickupBoxes.Add(dropoffBoxes);
				}
			}
			int totalUnits = job.TotalNeeded;
			pickupSummary.Add(new PickupSummaryItem(uniqueResourceName, pickupBoxes, totalUnits));
		}

		return (pickupSummary.OrderBy(s => s.Resource).ToList(), locationDrops);
	}
}