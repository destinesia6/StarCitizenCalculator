namespace StarCitizenCalculator;

using System.Collections.Generic;

/// <summary>
/// Model to hold the data collected from the TUI form.
/// </summary>
public class AppState
{
	// The current state of location names (Input 1)
	public List<string> LocationNames { get; set; } = ["", "", "", ""];

	// The list of jobs being collected (Input 2)
	public List<DeliveryJob> Jobs { get; set; } = [];

	// Temporary storage for the job currently being entered
	public DeliveryJob CurrentJob { get; set; } = new() { ResourceName = "" };

	// The current location index for which drop amounts are being entered
	public int CurrentLocationIndex { get; set; } = -1;

	// Application Stage
	public ApplicationStage Stage { get; set; } = ApplicationStage.InputLocations;
}

public enum ApplicationStage
{
	InputLocations,
	InputJobs,
	ShowManifest
}

/// <summary>
/// Represents a single delivery job for a resource.
/// </summary>
public class DeliveryJob
{
	public required string ResourceName { get; set; }
	public Dictionary<string, int> Drops { get; set; } = new();

	/// <summary>
	/// Calculates the total amount of the resource needed for this job.
	/// </summary>
	public int TotalNeeded => Drops.Values.Count > 0 ? Drops.Values.Sum() : 0;
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

	public void Add(BoxCount newBoxCount)
	{
		  Box1 += newBoxCount.Box1;
			Box2 += newBoxCount.Box2;
			Box4 += newBoxCount.Box4;
	}
}