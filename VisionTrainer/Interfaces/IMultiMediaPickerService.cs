using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisionTrainer.Models;

namespace VisionTrainer.Interfaces
{
	public interface IMultiMediaPickerService
	{
		event EventHandler<MediaDetails> OnMediaPicked;
		event EventHandler<IList<MediaDetails>> OnMediaPickedCompleted;
		Task<IList<MediaDetails>> PickPhotosAsync();
		Task<IList<MediaDetails>> PickVideosAsync();
	}
}
