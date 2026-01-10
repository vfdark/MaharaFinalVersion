// Tab switching
document.querySelectorAll('.nav-item[data-tab]').forEach(item => {
    item.addEventListener('click', function(e) {
        e.preventDefault();
        const tab = this.dataset.tab;
        document.querySelectorAll('.nav-item[data-tab]').forEach(nav => nav.classList.remove('active'));
        this.classList.add('active');
        document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));
        document.getElementById(tab).classList.add('active');
    });
});

// Modal promotion
document.addEventListener('DOMContentLoaded', function() {
    const modal = document.getElementById('promoteModal');
    const closeBtn = modal.querySelector('.close-btn');
    const cancelBtn = document.getElementById('cancelBtn');
    const studentName = document.getElementById('modal-student-name');
    const studentPoints = document.getElementById('modal-student-points');
    const studentSkills = document.getElementById('modal-student-skills');
    const studentId = document.getElementById('modal-student-id');

    document.querySelectorAll('.promote-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            studentName.textContent = this.dataset.name;
            studentPoints.textContent = this.dataset.points + " نقطة";
            studentSkills.innerHTML = '';
            this.dataset.skills.split(', ').forEach(skill => {
                const li = document.createElement('li');
                li.textContent = skill;
                studentSkills.appendChild(li);
            });
            studentId.value = this.dataset.id;
            modal.style.display = 'flex';
        });
    });

    closeBtn.addEventListener('click', () => modal.style.display = 'none');
    cancelBtn.addEventListener('click', () => modal.style.display = 'none');
    window.addEventListener('click', e => { if(e.target === modal) modal.style.display='none'; });
});
