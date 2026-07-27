using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class JournalEntriesViewModel : BaseViewModel
    {
        private readonly IJournalService _journalService;
        private readonly IDialogService _dialogService;

        public JournalEntriesViewModel(IJournalService journalService, IDialogService dialogService)
        {
            _journalService = journalService;
            _dialogService = dialogService;
        }

        [ObservableProperty] private ObservableCollection<JournalEntry> _journals = new();
        [ObservableProperty] private JournalEntry? _selectedJournal;

        public async Task LoadDataAsync()
        {
            var list = await _journalService.GetAllJournalsAsync();
            Journals.Clear();
            foreach (var item in list)
            {
                Journals.Add(item);
            }
        }

        [RelayCommand]
        private async Task ShowNewJournalDialog()
        {
            // For now, assume a transient JournalEditorViewModel is fetched by IDialogService or similar pattern
            var result = await _dialogService.ShowDialogAsync("JournalEditorDialog", null);
            if (result)
            {
                await LoadDataAsync();
            }
        }
    }
}
