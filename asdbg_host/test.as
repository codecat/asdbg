void main()
{
	int frameCount = 0;
	bool keepGoing = true;
	while (keepGoing) {
		frameCount++;
		print("Frame: " + frameCount);

		sleep(17);
	}
	print("it's the end");
}
