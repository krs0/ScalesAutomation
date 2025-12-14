namespace ScalesAutomation;

public partial class NextLotData : Form
{
    public delegate void CloseEvent();
    public CloseEvent WindowClosed;

    public delegate void ApplyClickedEvent();
    public CloseEvent ApplyClicked;

    public NextLotData()
    {
        InitializeComponent();
    }

    public LotInfo GetLotInfo()
    {
        return uctlLotData.LotInfo;
    }

    #region "Events"

    private void NextLotData_FormClosing(object sender, FormClosingEventArgs e)
    {
        WindowClosed();
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        ApplyClicked();
        this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    #endregion

}
