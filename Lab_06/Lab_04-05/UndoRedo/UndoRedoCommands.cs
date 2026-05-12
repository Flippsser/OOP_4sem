using System.Collections.ObjectModel;

namespace Lab_04_05
{
    public class AddServiceCommand : IUndoRedoCommand
    {
        private readonly ObservableCollection<DrivingService> _collection;
        private readonly DrivingService _service;
        private readonly MainViewModel _vm;
        public string Description { get; }

        public AddServiceCommand(ObservableCollection<DrivingService> collection, DrivingService service, MainViewModel vm, string desc)
        {
            _collection = collection;
            _service = service;
            _vm = vm;
            Description = desc;
        }

        public void Execute()
        {
            if (!_collection.Contains(_service))
                _collection.Add(_service);
            _vm?.RefreshAfterChange();
        }

        public void Undo()
        {
            _collection.Remove(_service);
            _vm?.RefreshAfterChange();
        }
    }

    public class DeleteServiceCommand : IUndoRedoCommand
    {
        private readonly ObservableCollection<DrivingService> _collection;
        private readonly DrivingService _service;
        private readonly int _index;
        private readonly MainViewModel _vm;
        public string Description { get; }

        public DeleteServiceCommand(ObservableCollection<DrivingService> collection, DrivingService service, int index, MainViewModel vm, string desc)
        {
            _collection = collection;
            _service = service;
            _index = index;
            _vm = vm;
            Description = desc;
        }

        public void Execute()
        {
            _collection.Remove(_service);
            _vm?.RefreshAfterChange();
        }

        public void Undo()
        {
            if (_index < _collection.Count)
                _collection.Insert(_index, _service);
            else
                _collection.Add(_service);
            _vm?.RefreshAfterChange();
        }
    }

    public class EditServiceCommand : IUndoRedoCommand
    {
        private readonly DrivingService _service;
        private readonly DrivingService _snapshot;
        private readonly DrivingService _edited;
        private readonly MainViewModel _vm;
        public string Description { get; }

        public EditServiceCommand(DrivingService service, DrivingService snapshot, DrivingService edited, MainViewModel vm, string desc)
        {
            _service = service;
            _snapshot = snapshot;
            _edited = edited;
            _vm = vm;
            Description = desc;
        }

        public void Execute()
        {
            CopyProperties(_edited, _service);
            _vm?.RefreshAfterChange();
        }

        public void Undo()
        {
            CopyProperties(_snapshot, _service);
            _vm?.RefreshAfterChange();
        }

        private static void CopyProperties(DrivingService from, DrivingService to)
        {
            to.FullName = from.FullName;
            to.Description = from.Description;
            to.Category = from.Category;
            to.Rating = from.Rating;
            to.Price = from.Price;
            to.Quantity = from.Quantity;
            to.TransmissionType = from.TransmissionType;
            to.DurationHours = from.DurationHours;
            to.Country = from.Country;
            to.Discount = from.Discount;
            to.IsNotAvailable = from.IsNotAvailable;
            to.PurchasedCount = from.PurchasedCount;
            to.Producer = from.Producer;
            to.Color = from.Color;
            to.GroupSize = from.GroupSize;
            to.ImagePaths = new System.Collections.Generic.List<string>(from.ImagePaths ?? new());
            to.RelatedServiceIds = new System.Collections.Generic.List<string>(from.RelatedServiceIds ?? new());
        }
    }

    public class PurchaseServiceCommand : IUndoRedoCommand
    {
        private readonly DrivingService _service;
        private readonly MainViewModel _vm;
        public string Description { get; }

        public PurchaseServiceCommand(DrivingService service, MainViewModel vm, string desc)
        {
            _service = service;
            _vm = vm;
            Description = desc;
        }

        public void Execute()
        {
            if (_service.Quantity > 0 && !_service.IsNotAvailable)
            {
                _service.Quantity--;
                _service.PurchasedCount++;
            }
            _vm?.RefreshAfterChange();
        }

        public void Undo()
        {
            _service.Quantity++;
            _service.PurchasedCount--;
            _vm?.RefreshAfterChange();
        }
    }
}
