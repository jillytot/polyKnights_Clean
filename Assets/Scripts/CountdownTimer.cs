using UnityEngine;

public class CountdownTimer {
	bool started;
	float seconds;
	float startTime;

	public CountdownTimer(float seconds) {
		this.seconds = seconds;
	}

	public bool Started {
		get {
			return started;
		}
	}

	public void Start() {
		if(started)
			return;

		started = true;
		startTime = Time.time;
	}

	public void Reset() {
		started = false;
		Start();
	}

	public int GetTimeInt() {
		if(TimeLeft() < 0)
			return 0;

		return (int)(TimeLeft() + 0.9999999f);
	}

	float TimeLeft() {
		return seconds - (Time.time - startTime);
	}
}