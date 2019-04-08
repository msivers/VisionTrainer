using System;
namespace VisionTrainer
{
	public interface IStatefulContent
	{
		void DidAppear();
		void DidDisappear();
	}
}
